using System.Linq;
using System.Reflection;
using WumpusLib.Game.GameModes;
using WumpusLib.Map;

namespace WumpusLib.Entities.Player.Bot;

public class EntityTracker
{
    private readonly Dictionary<Type, List<Type>?> _incompatibleSpawningObjectsCache = new();
    public Dictionary<int, List<Type>> ConfirmedEntityRooms = new();

    public HashSet<int> ConfirmedSafeRooms = new();

    public Dictionary<int, List<int>> RememberedRooms = new();

    public List<TrackedEntityInfo> TrackedEntityInfos;

    private readonly Dictionary<Type, float> _entitiesOfInterest = new();
    private GameMode GameMode { get; }

    public event EventHandler<ConfirmedEntityArgs>? OnEntityOfInterestLocationConfirmed;

    private readonly int _amountOfRooms;

    public EntityTracker(GameMode gameMode, int amountOfRooms)
    {
        GameMode = gameMode;
        _amountOfRooms = amountOfRooms;

        TrackedEntityInfos = GetTrackedEntityInfosFromGameMode(GameMode, amountOfRooms).ToList();

        //Mark separation distances for incompatible entities for filtering:
        foreach (var entityInfo in GameMode.EntitySpawnParameters)
        {
            var fieldInfo = entityInfo.EntityType.GetField("IncompatibleSpawningObjects",
                BindingFlags.Public | BindingFlags.Instance);

            _incompatibleSpawningObjectsCache[entityInfo.EntityType] =
                fieldInfo?.GetValue(entityInfo.EntityType) as List<Type>;
        }
    }

    public void RegisterEntityTypeOfInterest(Type entityType, float probabilityThreshold)
    {
        if (_entitiesOfInterest.ContainsKey(entityType))
        {
            throw new InvalidOperationException("The type is already registered.");
        }

        _entitiesOfInterest.Add(entityType, probabilityThreshold);
    }

    public void UnregisterEntityTypeOfInterest(Type entityType)
    {
        if (_entitiesOfInterest.ContainsKey(entityType))
        {
            _entitiesOfInterest.Remove(entityType);
        }
    }

    /// <summary>
    /// Updates the EntityTracker with the current room, room links, and nearby sensed entities. 
    /// This method processes the new information to update the tracked entities, safe rooms,
    /// confirmed entity rooms, and raises events for entities of interest.
    /// </summary>
    /// <param name="currentRoom">The current room number the player is in.</param>
    /// <param name="roomLinks">A list of room numbers connected to the current room.</param>
    /// <param name="nearbyEntities">A list of sensed nearby entities, with their types, distances, and strengths.</param>
    public void Update(int currentRoom, List<int> roomLinks, List<NearbyEntitySenseInfo> nearbyEntities)
    {
        // Update the remembered rooms with the current room and room links
        if (RememberedRooms.ContainsKey(currentRoom))
            RememberedRooms[currentRoom] = RememberedRooms[currentRoom].Union(roomLinks).ToList();
        else
            RememberedRooms.Add(currentRoom, roomLinks);

        // Update the tracked entity information based on the nearby entities
        if (nearbyEntities.Any())
            TrackedEntityInfos = UpdateTrackedEntityRoomGuesses(TrackedEntityInfos, nearbyEntities, currentRoom, ConfirmedSafeRooms, RememberedRooms);
        else
            // If no hazards or enemies are detected, mark all rooms and the current room as safe
            ConfirmedSafeRooms.UnionWith(roomLinks);

        // Update the confirmed entity rooms based on the tracked entity information and nearby entities
        ConfirmedEntityRooms = GetConfirmedEntityRooms(TrackedEntityInfos, nearbyEntities);

        // Raise events for entities of interest, if any
        RaiseEventsForEntitiesOfInterest(nearbyEntities);
    }

    private List<int> GetMostLikelyRooms(Type entityType)
    {
        // Retrieve the TrackedEntityInfo for the specified entityType
        var trackedEntityInfo = TrackedEntityInfos.FirstOrDefault(info => info.Type == entityType);

        // If no matching TrackedEntityInfo is found, return an empty list
        if (trackedEntityInfo == null || trackedEntityInfo.RoomProbabilities.Count == 0)
        {
            return new List<int>();
        }

        // Find the highest probability value
        var maxProbability = trackedEntityInfo.RoomProbabilities.Max(x => x.Value);

        // Get the list of room numbers with the highest probability
        var mostLikelyRooms = trackedEntityInfo.RoomProbabilities
            .Where(x => x.Value == maxProbability)
            .Select(x => x.Key)
            .ToList();

        return mostLikelyRooms;
    }

    public List<TrackedEntityInfo> UpdateTrackedEntityRoomGuesses(
        List<TrackedEntityInfo> possibleEntityGuesses,
        IEnumerable<NearbyEntitySenseInfo> nearbyEntities,
        int currentRoom,
        IReadOnlySet<int> roomLinks,
        IDictionary<int, List<int>> rememberedRooms)
    {
        var sensedEntities = GetSensedEntities(nearbyEntities);

        foreach (var entityType in sensedEntities)
        {
            possibleEntityGuesses = HandleSensedEntity(possibleEntityGuesses, entityType, roomLinks, rememberedRooms, currentRoom);
        }

        possibleEntityGuesses = HandleDisappearedEntities(possibleEntityGuesses, sensedEntities, roomLinks, currentRoom);

        return possibleEntityGuesses;
    }

    private List<Type> GetSensedEntities(IEnumerable<NearbyEntitySenseInfo> nearbyEntities)
    {
        return nearbyEntities
            .Where(x => x.Strength == 1 && x.Distance == 1)
            .Select(x => x.Entity)
            .ToList();
    }

    private List<TrackedEntityInfo> HandleSensedEntity(
        List<TrackedEntityInfo> possibleEntityGuesses,
        Type entityType,
        IReadOnlySet<int> roomLinks,
        IDictionary<int, List<int>> rememberedRooms,
        int currentRoom)
    {
        var entitySpawnParameters = GameMode.EntitySpawnParameters.First(x => x.EntityType == entityType);
        var separationDistance = entitySpawnParameters.DegreeOfSpawnSeparation;

        var validRoomsWithinSeparation = GetValidRoomsWithinSeparation(currentRoom, roomLinks, rememberedRooms, separationDistance);

        var trackedEntity = possibleEntityGuesses.FirstOrDefault(x => x.Type == entityType);

        if (trackedEntity != null)
        {
            possibleEntityGuesses = UpdateTrackedEntity(possibleEntityGuesses, trackedEntity, entityType, separationDistance, validRoomsWithinSeparation, rememberedRooms);
        }
        else
        {
            possibleEntityGuesses = AddNewTrackedEntity(possibleEntityGuesses, entityType, validRoomsWithinSeparation);
        }

        return possibleEntityGuesses;
    }

    private static List<int> GetValidRoomsWithinSeparation(int currentRoom, IReadOnlySet<int> roomLinks, IDictionary<int, List<int>> rememberedRooms, int separationDistance)
    {
        return roomLinks
            .Where(x => Extensions.DijkstraGetShortestPathTo(currentRoom, x, rememberedRooms).Count - 1 <= separationDistance)
            .ToList();
    }

    private List<TrackedEntityInfo> UpdateTrackedEntity(
        List<TrackedEntityInfo> possibleEntityGuesses,
        TrackedEntityInfo trackedEntity,
        Type entityType,
        int separationDistance,
        IEnumerable<int> validRoomsWithinSeparation,
        IDictionary<int, List<int>> rememberedRooms)
    {
        var intersectedRooms = validRoomsWithinSeparation.Intersect(trackedEntity.PossibleRooms).ToList();
        var possibleRooms = GetValidRoomsForEntity(intersectedRooms, entityType, separationDistance, ConfirmedEntityRooms, rememberedRooms);
        var probabilities = GenerateEqualProbabilities(possibleRooms);

        var updatedEntity = new TrackedEntityInfo(entityType, possibleRooms, probabilities, _amountOfRooms);

        // Update the entity in the list
        var entityIndex = possibleEntityGuesses.IndexOf(trackedEntity);
        if (entityIndex != -1)
        {
            possibleEntityGuesses[entityIndex] = updatedEntity;
        }

        return possibleEntityGuesses;
    }

    private List<TrackedEntityInfo> AddNewTrackedEntity(
        List<TrackedEntityInfo> possibleEntityGuesses,
        Type entityType,
        List<int> validRoomsWithinSeparation)
    {
        var probabilities = GenerateEqualProbabilities(validRoomsWithinSeparation);

        var trackedEntity = new TrackedEntityInfo(entityType, validRoomsWithinSeparation, probabilities, _amountOfRooms);
        possibleEntityGuesses.Add(trackedEntity);

        return possibleEntityGuesses;
    }

    private static Dictionary<int, double> GenerateEqualProbabilities(List<int> rooms)
    {
        var probabilities = new Dictionary<int, double>();
        foreach (var room in rooms)
        {
            probabilities[room] = 1.0 / rooms.Count;
        }

        return probabilities;
    }

    private List<TrackedEntityInfo> HandleDisappearedEntities(
        IEnumerable<TrackedEntityInfo> possibleEntityGuesses,
        ICollection<Type> sensedEntities,
        IReadOnlySet<int> roomLinks,
        int currentRoom)
    {
        var disappearedEntities = possibleEntityGuesses
            .Where(x => !sensedEntities.Contains(x.Type))
            .ToList();

        var updatedDisappearedEntities = UpdateDisappearedEntities(disappearedEntities, roomLinks, currentRoom);

        return updatedDisappearedEntities;
    }

    private List<TrackedEntityInfo> UpdateDisappearedEntities(
        List<TrackedEntityInfo> disappearedEntities,
        IReadOnlySet<int> roomLinks,
        int currentRoom)
    {
        var updatedDisappearedEntities = new List<TrackedEntityInfo>();

        foreach (var disappearedEntity in disappearedEntities)
        {
            var updatedPossibleRooms = roomLinks.Except(new[] { currentRoom }).ToList();
            var probabilities = GenerateEqualProbabilities(updatedPossibleRooms);

            var newEntity = new TrackedEntityInfo(disappearedEntity.Type, updatedPossibleRooms, probabilities, _amountOfRooms);
            updatedDisappearedEntities.Add(newEntity);
        }

        return updatedDisappearedEntities;
    }


    public List<int> GetValidRoomsForEntity(IEnumerable<int> intersectedRooms, Type entityType, int separationDistance,
        IDictionary<int, List<Type>> confirmedEntityRooms, IDictionary<int, List<int>> rememberedRooms)
    {
        var validRooms = new List<int>();

        foreach (var room in intersectedRooms)
        {
            if (_incompatibleSpawningObjectsCache.TryGetValue(entityType, out var incompatibleEntities) &&
                confirmedEntityRooms.TryGetValue(room, out var confirmedEntitiesInRoom))
            {
                var isRoomValid = true;

                foreach (var confirmedEntity in confirmedEntitiesInRoom)
                {
                    if (!incompatibleEntities.Contains(confirmedEntity)) continue;

                    var pathLength = Extensions.DijkstraGetShortestPathTo(room,
                        confirmedEntityRooms.Single(x => x.Value.Contains(confirmedEntity)).Key, rememberedRooms).Count - 1;

                    if (pathLength > separationDistance)
                        continue;

                    isRoomValid = false;
                    break;
                }

                if (isRoomValid) validRooms.Add(room);
            }
            else
            {
                validRooms.Add(room);
            }
        }

        return validRooms;
    }

    public static Dictionary<int, List<Type>> GetConfirmedEntityRooms(List<TrackedEntityInfo> trackedEntityInfos,
        IReadOnlyCollection<NearbyEntitySenseInfo> nearbyEntities)
    {
        var confirmedEntityRooms = new Dictionary<int, List<Type>>();

        foreach (var trackedEntity in trackedEntityInfos)
        {
            if (nearbyEntities.Count(e => e.Entity == trackedEntity.Type) < trackedEntity.PossibleRooms.Count)
                continue;

            foreach (var room in trackedEntity.PossibleRooms)
                if (!confirmedEntityRooms.ContainsKey(room))
                    confirmedEntityRooms[room] = new List<Type> {trackedEntity.Type};
                else
                    confirmedEntityRooms[room].Add(trackedEntity.Type);
        }

        return confirmedEntityRooms;
    }

    public void RaiseEventsForEntitiesOfInterest(IReadOnlyCollection<NearbyEntitySenseInfo> nearbyEntities)
    {
        ConfirmedEntityRooms = GetConfirmedEntityRooms(TrackedEntityInfos, nearbyEntities);

        foreach (var entityType in _entitiesOfInterest)
        {
            if (ConfirmedEntityRooms.Any(x => x.Value.Contains(entityType.Key)))
            {
                var roomNumber = ConfirmedEntityRooms.First(x => x.Value.Contains(entityType.Key)).Key;
                OnEntityOfInterestLocationConfirmed?.Invoke(this, new ConfirmedEntityArgs(roomNumber, entityType.Key));
            }
            else
            {
                var probableRooms = GetRoomsWithinProbabilityRange(entityType.Key, entityType.Value, 1);

                if (probableRooms.Count <= 0)
                    continue;

                foreach (var roomNumber in probableRooms)
                {
                    var probability = TrackedEntityInfos.First(x => x.Type == entityType.Key).RoomProbabilities[roomNumber];
                    OnEntityOfInterestLocationConfirmed?.Invoke(this, new ConfirmedEntityArgs(roomNumber, entityType.Key, probability));
                }
            }
        }
    }

    public List<int> GetRoomsWithinProbabilityRange(Type entityType, double lowerBound, double upperBound)
    {
        var trackedEntity = TrackedEntityInfos.FirstOrDefault(x => x.Type == entityType);

        if (trackedEntity == null)
        {
            return new List<int>();
        }

        var roomsWithinRange = trackedEntity.RoomProbabilities
            .Where(kvp => kvp.Value >= lowerBound && kvp.Value <= upperBound)
            .Select(kvp => kvp.Key)
            .ToList();

        return roomsWithinRange;
    }

    public bool IsRoomLikelyToContain(Type entityType, int roomNumber, double probabilityThreshold = 0.5)
    {
        // Retrieve the TrackedEntityInfo for the specified entityType
        var trackedEntityInfo = TrackedEntityInfos.FirstOrDefault(info => info.Type == entityType);

        // If no matching TrackedEntityInfo is found or the roomNumber is not in RoomProbabilities, return false
        if (trackedEntityInfo == null || !trackedEntityInfo.RoomProbabilities.ContainsKey(roomNumber))
        {
            return false;
        }

        // Check the probability of the specified roomNumber containing the entity
        double probability = trackedEntityInfo.RoomProbabilities[roomNumber];

        // Return true if the probability is above the threshold, false otherwise
        return probability > probabilityThreshold;
    }

    protected static IEnumerable<TrackedEntityInfo> GetTrackedEntityInfosFromGameMode(GameMode gameMode,
        int amountOfRooms)
    {
        return gameMode.EntitySpawnParameters.SelectMany(spawnParameters => Enumerable.Range(1, spawnParameters.AmountToSpawn)
            .Select(_ => new TrackedEntityInfo(spawnParameters.EntityType, amountOfRooms)));
    }

    private int? GetRoomNumberForEntityType(Type entityType)
    {
        return ConfirmedEntityRooms.FirstOrDefault(kvp => kvp.Value.Contains(entityType)).Key;
    }

}
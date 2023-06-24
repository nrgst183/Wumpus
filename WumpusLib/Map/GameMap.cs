using System.Collections.Concurrent;
using WumpusLib.Entities;
using WumpusLib.Entities.Events;
using WumpusLib.Game.GameModes;
using WumpusLib.Map.Events;
using WumpusLib.Map.Results;

namespace WumpusLib.Map;

public abstract class GameMap
{
    /// <summary>
    ///     Room => entities mapping
    /// </summary>
    public readonly ConcurrentDictionary<int, List<Entity>> RoomEntityLinks = new();

    /// <summary>
    ///     Room => room links mappings
    /// </summary>
    public readonly ConcurrentDictionary<int, List<int>> RoomLinks = new();

    private bool _gameStarted;

    protected GameMap(int amountOfRooms)
    {
        AmountOfRooms = amountOfRooms;
        InitializeRoomDictionaries(AmountOfRooms);
    }

    public int AmountOfRooms { get; }
    public bool IsMapGenerated { get; protected set; }

    public bool GameStarted
    {
        get => _gameStarted;
        set
        {
            _gameStarted = value;

            if (value)
                OnGameStarted?.Invoke(this, EventArgs.Empty);
            else
                OnGameEnded?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler<GameMapUpdateArgs>? OnGameMapEvent;
    public event EventHandler<EntityDiedArgs>? OnEntityKilled;
    public event EventHandler<EntityUpdateArgs>? OnEntitySpawned;
    public event EventHandler<EventArgs>? OnGameStarted;
    public event EventHandler<EventArgs>? OnGameEnded;

    protected List<EntitySpawnParameters> EntitySpawnParameters = new();
    public void InitializeGameMap(GameMode gameType)
    {
        BuildMapLayout();
        CreateAndAddEntitiesToRooms(gameType.EntitySpawnParameters);
        IsMapGenerated = true;
    }

    protected abstract void BuildMapLayout();

    private void InitializeRoomDictionaries(int amountOfRooms)
    {
        for (var room = 1; room <= amountOfRooms; room++)
        {
            RoomLinks.TryAdd(room, new List<int>());
            RoomEntityLinks.TryAdd(room, new List<Entity>());
        }
    }

    /// <summary>
    ///     Creates and places entities from collection of EntitySpawnParameters
    /// </summary>
    /// <param name="spawnParameters">The entity types to create</param>
    public void CreateAndAddEntitiesToRooms(List<EntitySpawnParameters> spawnParameters)
    {
        foreach (var type in spawnParameters)
        {
            EntitySpawnParameters.Add(type);

            for (var counter = 0; counter < type.AmountToSpawn; counter++)
            {
                var accessor = new EntityGameMapAccessor(this);
                var entity = Extensions.CreateEntityFromType(type.EntityType, accessor);
                AddEntityToRandomRoom(entity, type.DegreeOfSpawnSeparation);
            }
        }
    }

    /// <summary>
    ///     Places an entity in a random room. Checks for minimum distance away from incompatible entities.
    /// </summary>
    /// <param name="entity">The entity to spawn</param>
    /// <param name="spawnDegreeOfSeparation">Minimum separation distance from other incompatible entities</param>
    /// <returns>MoveResult indicating whether the placement was successful</returns>
    public MoveResult AddEntityToRandomRoom(Entity entity, int spawnDegreeOfSeparation)
    {
        var incompatibleRooms = GetEntityIncompatibleRooms(entity).ToList();

        if (incompatibleRooms.Count >= AmountOfRooms)
            return MoveResult.NoCompatibleRoomsToMoveTo;

        // Initialize separateEnoughRooms as a HashSet with all rooms
        var separateEnoughRooms = new HashSet<int>(RoomLinks.Keys);

        foreach (var incompatibleRoom in incompatibleRooms)
        {
            // Get all rooms at a good distance away from incompatible one
            var roomsSeparateFromEntity = Extensions.BfsTraverseNodesByDegreesOfSeparation(incompatibleRoom, RoomLinks,
                spawnDegreeOfSeparation).Keys;

            // Remove current room and rooms not contained in current traversal
            separateEnoughRooms.Remove(incompatibleRoom);
            separateEnoughRooms.IntersectWith(roomsSeparateFromEntity);
        }

        // If there are no separate enough rooms left, return an error
        if (separateEnoughRooms.Count == 0)
            return MoveResult.NoCompatibleRoomsToMoveTo;

        // Select a random compatible room
        var selectedRoom = separateEnoughRooms.ElementAt(Extensions.Random.Next(separateEnoughRooms.Count));

        var internalResult = InternalMoveTo(entity, selectedRoom);

        if (internalResult != MoveResult.Success)
            return internalResult;

        OnEntitySpawned?.Invoke(this, new EntityUpdateArgs(entity));
        entity.OnKilled += Entity_OnKilled;

        return internalResult;
    }

    public MoveResult AddEntityToMap(Entity entity, int room)
    {
        if (entity == null) throw new ArgumentNullException($"Map.PlaceEntityOnMap: Entity {entity} was null.");

        //if room doesn't exist
        if (!RoomLinks.ContainsKey(room)) return MoveResult.InvalidRoomNumber;

        if (IsEntityPlacedOnMap(entity))
        {
            return MoveResult.EntityAlreadyOnMap;
        }

        var result = InternalMoveTo(entity, room);

        OnGameMapEvent?.Invoke(this, new GameMapUpdateArgs(entity, room));

        return result;
    }

    public int GetRoomNumberForEntity(Entity entity)
    {
        return (from room in RoomEntityLinks where room.Value.Contains(entity) select room.Key).FirstOrDefault();
    }

    public IEnumerable<int> GetRoomLinksForEntity(Entity entity)
    {
        var room = GetRoomNumberForEntity(entity);

        return room != 0 ? RoomLinks[room] : new List<int>();
    }

    public bool IsEntityPlacedOnMap(Entity entity)
    {
        var room = GetRoomNumberForEntity(entity);
        return room != 0 && RoomLinks.ContainsKey(room);
    }

    public List<NearbyEntitySenseInfo> GetNearbyEntitiesAndBroadcastStrength(int room)
    {
        var entitiesDict = new List<NearbyEntitySenseInfo>();

        foreach (var ent in RoomEntityLinks.Values.SelectMany(x => x))
        {
            var currentRoom = GetRoomNumberForEntity(ent);

            // Perform BFS traversal once for each entity, considering the maximum sense value
            var roomLinksAtDepth = Extensions.BfsTraverseNodesByDegreesOfSeparation(currentRoom, RoomLinks, ent.EntitySenseBroadcastSize, true);

            // Check if the room is reachable within the sense range of the entity
            if (!roomLinksAtDepth.TryGetValue(room, out var distance)) 
                continue;

            if (distance <= ent.EntitySenseBroadcastSize)
            {
                entitiesDict.Add(new NearbyEntitySenseInfo(ent.GetType(), ent.EntitySenseBroadcastSize, ent.EntitySenseBroadcastSize + 1 - distance));
            }
        }

        return entitiesDict;
    }

    /// <summary>
    ///     Moves an entity to a chosen room
    /// </summary>
    /// <param name="entity">The entity to be moved</param>
    /// <param name="room">Room number</param>
    /// <returns>MoveResult indicating the result of the move attempt.</returns>
    /// <exception cref="ArgumentNullException">Throws an exception if the entity is null</exception>
    public MoveResult MoveTo(Entity entity, int room)
    {
        if (entity == null) throw new ArgumentNullException($"Map.MoveTo: Entity {entity} was null.");

        //check if entity is allowed to move
        if (!entity.CanMove || entity.IsDead) return MoveResult.EntityNotAllowedToMove;

        //if room doesn't exist
        if (!RoomLinks.ContainsKey(room)) return MoveResult.InvalidRoomNumber;

        //then check if this room is the same one
        if (RoomEntityLinks[room].Contains(entity)) return MoveResult.CannotMoveToSameRoom;

        if (!IsEntityPlacedOnMap(entity)) return MoveResult.EntityNotOnMap;

        var originalRoom = GetRoomNumberForEntity(entity);

        if (!entity.CanMoveToAnyRoom && !RoomLinks[originalRoom].Contains(room))
            return MoveResult.IllegalMove;

        var result = InternalMoveTo(entity, originalRoom, room);

        if (!GameStarted)
            return result;

        //if entity is moving rooms, get original room and fire event with both rooms, else just fire new room
        OnGameMapEvent?.Invoke(this,
            IsEntityPlacedOnMap(entity)
                ? new GameMapUpdateArgs(entity, originalRoom, room)
                : new GameMapUpdateArgs(entity, room));

        return result;
    }

    /// <summary>
    ///     Moves an entity to a chosen room
    /// </summary>
    /// <param name="primaryEntity">The controlling entity</param>
    /// <param name="entityToMove">The entity to be moved</param>
    /// <param name="room">Desired room number</param>
    /// <returns>MoveResult indicating the result of the move attempt.</returns>
    /// <exception cref="ArgumentNullException">Throws exception on either entities being null.</exception>
    public MoveResult MoveEntityTo(Entity primaryEntity, Entity entityToMove, int room)
    {
        if (primaryEntity == null)
            throw new ArgumentNullException($"Map.MoveEntityTo: Primary Entity: {primaryEntity} was null.");
        if (entityToMove == null)
            throw new ArgumentNullException($"Map.MoveEntityTo: Secondary Entity: {entityToMove} was null.");

        //first check if primary is allowed to move this entity
        if (!primaryEntity.CanMoveOtherEntities && !entityToMove.IsWeapon && !entityToMove.IsEquippable)
            return MoveResult.EntityCannotMoveOtherEntities;

        //check if entity can move
        if (!entityToMove.CanMove || entityToMove.IsDead)
            return MoveResult.EntityNotAllowedToMove;

        //check if the room exists
        if (!RoomLinks.ContainsKey(room)) return MoveResult.IllegalMove;

        //then check if this room is the same one
        if (RoomEntityLinks[room].Contains(entityToMove)) return MoveResult.CannotMoveToSameRoom;

        var originalRoom = GetRoomNumberForEntity(entityToMove);

        //if primary entity can move entities to any room (super-bats), let them move without verifying room links are connected.
        if (primaryEntity.CanMoveEntitiesToAnyRoom) return InternalMoveTo(entityToMove, originalRoom, room);

        //ensure room is connected to chosen room
        if (!primaryEntity.CanMoveToAnyRoom && !RoomLinks.ContainsKey(room)) return MoveResult.IllegalMove;

        var result = InternalMoveTo(entityToMove, originalRoom, room);

        //fire move event for game state/observables: 
        if (GameStarted)
        {
            OnGameMapEvent?.Invoke(this, new GameMapUpdateArgs(primaryEntity, entityToMove, originalRoom, room));
        }

        return result;
    }

    /// <summary>
    ///     Picks up and removes an equippable entity from its room.
    /// </summary>
    /// <param name="primaryEntity"></param>
    /// <param name="entityToEquip"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public EquipResult RemoveEntity(Entity primaryEntity, Entity entityToEquip)
    {
        if (primaryEntity == null)
            throw new ArgumentNullException($"Map.EquipEntityToInventory: Primary Entity: {primaryEntity} was null.");
        if (entityToEquip == null)
            throw new ArgumentNullException($"Map.EquipEntityToInventory: Secondary Entity: {entityToEquip} was null.");

        if (!IsEntityPlacedOnMap(entityToEquip)) return EquipResult.EntityNotOnMap;

        var primaryRoom = GetRoomNumberForEntity(primaryEntity);
        var equipRoom = GetRoomNumberForEntity(entityToEquip);

        if (primaryRoom != equipRoom && !primaryEntity.CanSeeAllRooms) return EquipResult.EntityNotInCurrentRoom;

        if (!entityToEquip.IsEquippable) return EquipResult.EntityNotEquippable;

        InternalRemoveEntity(entityToEquip);

        entityToEquip.OnKilled -= Entity_OnKilled;

        OnGameMapEvent?.Invoke(this, new GameMapUpdateArgs(primaryEntity, entityToEquip, primaryRoom, 0));

        return EquipResult.Success;
    }

    /// <summary>
    ///     Deploys an equippable entity into the primary entity's room
    /// </summary>
    /// <param name="primaryEntity"></param>
    /// <param name="equippable"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public MoveResult DeployEntity(Entity primaryEntity, Entity equippable)
    {
        if (primaryEntity == null)
            throw new ArgumentNullException($"Map.EquipEntityToInventory: Primary Entity: {primaryEntity} was null.");
        if (equippable == null)
            throw new ArgumentNullException($"Map.EquipEntityToInventory: Secondary Entity: {equippable} was null.");

        if (IsEntityPlacedOnMap(equippable)) return MoveResult.CannotMoveToSameRoom;

        var primaryRoom = GetRoomNumberForEntity(primaryEntity);

        InternalMoveTo(equippable, primaryRoom);

        equippable.OnKilled += Entity_OnKilled;

        OnGameMapEvent?.Invoke(this, new GameMapUpdateArgs(primaryEntity, equippable, 0, primaryRoom));

        return MoveResult.Success;
    }

    public IEnumerable<int> GetEntityIncompatibleRooms(Entity entity)
    {
        var entityType = entity.GetType();
        var entitySpawnParameters = EntitySpawnParameters.FirstOrDefault(x => x.EntityType == entityType);

        if (entitySpawnParameters == null)
        {
            return Enumerable.Empty<int>();
        }

        var incompatibleEntityTypes = entitySpawnParameters.IncompatibleSpawningObjects;

        return from room in RoomEntityLinks.Where(x => x.Value.Any())
            from ent in room.Value
            where incompatibleEntityTypes.Contains(ent.GetType())
            select room.Key;
    }

    public IEnumerable<T> GetEntitiesOnMapByType<T>(bool includeDeadEntities)
    {
        return includeDeadEntities
            ? RoomEntityLinks.Values.SelectMany(x => x).Where(entity => !entity.IsDead)
                .OfType<T>()
            : RoomEntityLinks.Values.SelectMany(x => x).OfType<T>();
    }

    public List<Entity> GetEntitiesInCurrentRoom(Entity entity)
    {
        var room = GetRoomNumberForEntity(entity);

        return RoomEntityLinks.ContainsKey(room) ? RoomEntityLinks[room] : new List<Entity>();
    }

    protected LinkResult LinkRooms(int room1, int room2)
    {
        if (RoomLinks[room1].Contains(room2) || RoomLinks[room2].Contains(room1)) return LinkResult.RoomsAlreadyLinked;

        InternalLinkRooms(room1, room2);

        return LinkResult.Success;
    }

    private void Entity_OnKilled(object? sender, EntityDiedArgs e)
    {
        //forward event on to all listeners:
        OnEntityKilled?.Invoke(this, e);
    }

    #region internal map functions

    //These functions are not checked because the checking/access is done in the protected/public functions instead
    private void InternalLinkRooms(int room1, int room2)
    {
        RoomLinks[room2].Add(room1);
        RoomLinks[room1].Add(room2);
    }

    private MoveResult InternalMoveTo(Entity entity, int roomMoveFrom, int roomMoveTo)
    {
        RoomEntityLinks[roomMoveFrom].Remove(entity);
        RoomEntityLinks[roomMoveTo].Add(entity);

        return MoveResult.Success;
    }

    private MoveResult InternalMoveTo(Entity entity, int room)
    {
        RoomEntityLinks[room].Add(entity);
        return MoveResult.Success;
    }

    private RemoveResult InternalRemoveEntity(Entity entity)
    {
        var room = GetRoomNumberForEntity(entity);

        RoomEntityLinks[room].Remove(entity);

        return RemoveResult.Success;
    }

    #endregion
}
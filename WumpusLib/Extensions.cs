using System.Reflection;
using WumpusLib.Entities;
using WumpusLib.Map;

namespace WumpusLib;

public static class Extensions
{
    public const string RandomCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    public static Random Random = new(Guid.NewGuid().GetHashCode());

    public static string GenerateRandomAlphanumericString(Random random, int length)
    {
        return new string(Enumerable.Repeat(RandomCharacters, length).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static string GenerateRandomString(Random random, int length, string characters)
    {
        return new string(Enumerable.Repeat(characters, length).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static bool IsEntityIncompatibleWith(EntitySpawnParameters first, EntitySpawnParameters second)
    {
        return first.IncompatibleSpawningObjects.Contains(second.EntityType)
               || second.IncompatibleSpawningObjects.Contains(first.EntityType)
               || first.IncompatibleSpawningObjects.Any(x => x.IsInstanceOfType(second.EntityType))
               || second.IncompatibleSpawningObjects.Any(x => x.IsInstanceOfType(first.EntityType));
    }

    public static string GetObscuredStringByTimeElapsed(string arrowName, DateTime lastFired, int seconds,
        char maskChar)
    {
        var secondsSinceFired = (int) DateTime.Now.Subtract(lastFired).TotalSeconds;

        var difference = secondsSinceFired % seconds;

        if (difference == 0)
            return arrowName;

        var str = arrowName.ToCharArray();

        for (var secs = secondsSinceFired; secs < difference; secs += seconds)
            ReplaceRandomCharacter(str, maskChar);

        return new string(str);
    }

    public static Dictionary<int, int> BfsTraverseNodesByDegreesOfSeparation(int root,
        IDictionary<int, List<int>> links,
        int separationDepth, bool onlyAtSeparationDepth = false)
    {
        var q = new Queue<int>();

        var seenList = new Dictionary<int, int>();

        q.Enqueue(root);

        seenList.Add(root, 0);

        while (q.Count > 0)
        {
            var current = q.Dequeue();

            if (!links.ContainsKey(current))
                continue;

            foreach (var neighbor in links[current])
            {
                // if neighbor is marked already, skip
                if (seenList.ContainsKey(neighbor))
                    continue;

                q.Enqueue(neighbor);

                // level of neighbor is level of current + 1
                seenList[neighbor] = seenList[current] + 1;
            }
        }

        return onlyAtSeparationDepth
            ? seenList.Where(x => x.Value == separationDepth).ToDictionary(i => i.Key, i => i.Value)
            : seenList;
    }

    public static List<int> DijkstraGetShortestPathTo(int start, int end, IDictionary<int, List<int>> links,
        List<int> excludeList = null)
    {
        var q = new Queue<int>();

        var distances = new Dictionary<int, int>();
        var previous = new Dictionary<int, int>();

        foreach (var node in links.Keys)
        {
            distances[node] = int.MaxValue;
            previous[node] = -1;
        }

        // Add this block to initialize missing nodes
        foreach (var node in links.Values.SelectMany(x => x).Distinct())
            if (!distances.ContainsKey(node))
            {
                distances[node] = int.MaxValue;
                previous[node] = -1;
            }

        distances[start] = 0;

        q.Enqueue(start);

        while (q.Count > 0)
        {
            var current = q.Dequeue();

            if (!links.TryGetValue(current, out var currentNeighbors)) continue;

            foreach (var neighbor in currentNeighbors.Where(neighbor =>
                         excludeList == null || !excludeList.Contains(neighbor)))
            {
                var tentativeDistance = distances[current] + 1;

                if (tentativeDistance >= distances[neighbor])
                    continue;

                distances[neighbor] = tentativeDistance;
                previous[neighbor] = current;

                // Only enqueue neighbor if it is not excluded
                if (excludeList == null || !excludeList.Contains(neighbor))
                    q.Enqueue(neighbor);
            }
        }


        if (previous[end] == -1) return new List<int>(); // Return an empty queue if there's no valid path.

        // build the path queue
        var path = new Queue<int>();
        var currentNode = end;

        while (currentNode != -1 && currentNode != start)
        {
            path.Enqueue(currentNode);
            currentNode = previous[currentNode];
        }

        if (currentNode == start)
            path.Enqueue(currentNode);

        return new List<int>(path.Reverse());
    }

    public static int GreatestCommonDivisor(int a, int b)
    {
        while (a != 0 && b != 0)
            if (a > b)
                a %= b;
            else
                b %= a;

        return a | b;
    }

    public static void ReplaceRandomCharacter(IList<char> arr, char mask)
    {
        var randIndex = Random.Next(arr.Count);
        arr[randIndex] = mask;
    }

    public static Entity CreateEntityFromType(Type type, EntityGameMapAccessor gameMapAccessor)
    {
        var constructor = type.GetConstructors().FirstOrDefault(x =>
            x.GetParameters().Length == 1 &&
            x.GetParameters().FirstOrDefault()?.ParameterType == typeof(EntityGameMapAccessor));

        if (constructor != null) return (Entity) Activator.CreateInstance(type, gameMapAccessor);

        var constructors = type.GetConstructors();

        foreach (var ctor in constructors)
        {
            var amountOfRooms = GetConstructorArgumentPositionByName(ctor, typeof(int), "amountOfRooms");
            var map = GetConstructorArgumentPositionByType(ctor, typeof(EntityGameMapAccessor));

            if (amountOfRooms != 0 || map != 1)
                continue;

            var entity = (Entity) Activator.CreateInstance(type, gameMapAccessor.AmountOfRooms, gameMapAccessor);
            gameMapAccessor.LinkEntityToAccessor(entity);
            return entity;
        }

        throw new InvalidOperationException($"Couldn't find entity constructor for type: {type}");
    }

    public static int GetConstructorArgumentPositionByName(ConstructorInfo info, Type type, string name)
    {
        var parameters = info.GetParameters();

        for (var counter = 0; counter < parameters.Length; counter++)
            if (parameters[counter].Name == name && parameters[counter].ParameterType == type)
                return counter;

        return -1;
    }

    public static int GetConstructorArgumentPositionByType(ConstructorInfo info, Type type)
    {
        var parameters = info.GetParameters();

        for (var counter = 0; counter < parameters.Length; counter++)
            if (parameters[counter].ParameterType == type)
                return counter;

        return -1;
    }

    public static IEnumerable<Type> GetAllTypes<T>()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(domainAssembly => domainAssembly.GetTypes())
            .Where(type => typeof(T).IsAssignableFrom(type));
    }

    public static IEnumerable<Type> GetAllTypesExcludingBase<T>()
    {
        return GetAllTypes<T>().Where(x => x != typeof(T));
    }
}
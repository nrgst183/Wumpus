namespace WumpusLib.Entities.Player.Bot;

public class RoomPathfinder
{
    protected List<int> ExploredRooms = new();
    protected Dictionary<int, List<int>> RememberedRoomsAndRoomLinks = new();
    protected HashSet<int> RoomExclusions = new();

    protected Dictionary<int, int> RoomVisitCounts = new();

    public RoomPathfinder(int currentRoom, List<int> roomLinks)
    {
        Update(currentRoom, roomLinks);
    }

    private int CurrentRoom { get; set; }

    public void Update(int currentRoom, List<int> roomLinks)
    {
        CurrentRoom = currentRoom;

        RememberedRoomsAndRoomLinks.Add(currentRoom, roomLinks);

        ExploredRooms.Add(currentRoom);

        if (RoomVisitCounts.ContainsKey(currentRoom))
            RoomVisitCounts[currentRoom]++;
        else
            RoomVisitCounts.Add(currentRoom, 1);
    }

    public void MarkRoomAsExcluded(int room)
    {
        RoomExclusions.Add(room);
    }

    public void UnmarkExcludedRoom(int room)
    {
        RoomExclusions.Remove(room);
    }

    public int FindNearestVisitedRoom()
    {
        var nearestVisitedRoom = -1;
        var shortestPathLength = int.MaxValue;

        foreach (var visitedRoom in ExploredRooms)
        {
            // Skip the current room
            if (visitedRoom == CurrentRoom)
                continue;

            var pathToVisitedRoom =
                Extensions.DijkstraGetShortestPathTo(CurrentRoom, visitedRoom, RememberedRoomsAndRoomLinks,
                    RoomExclusions.ToList());

            if (pathToVisitedRoom.Count >= shortestPathLength)
                continue;

            nearestVisitedRoom = visitedRoom;
            shortestPathLength = pathToVisitedRoom.Count;
        }

        return nearestVisitedRoom;
    }

    public int FindNearestUnvisitedRoom()
    {
        var nearestUnvisitedRoom = -1;
        var shortestPathLength = int.MaxValue;

        // Get the list of unvisited rooms by excluding the explored rooms from the total rooms
        var unvisitedRooms = Enumerable.Range(1, RememberedRoomsAndRoomLinks.Keys.Count).Except(ExploredRooms).ToList();

        foreach (var unvisitedRoom in unvisitedRooms)
        {
            // Skip the current room
            if (unvisitedRoom == CurrentRoom)
                continue;

            var pathToUnvisitedRoom =
                Extensions.DijkstraGetShortestPathTo(CurrentRoom, unvisitedRoom, RememberedRoomsAndRoomLinks,
                    RoomExclusions.ToList());

            if (pathToUnvisitedRoom.Count >= shortestPathLength)
                continue;

            nearestUnvisitedRoom = unvisitedRoom;
            shortestPathLength = pathToUnvisitedRoom.Count;
        }

        return nearestUnvisitedRoom;
    }

    public int FindLeastVisitedRoom()
    {
        return RoomVisitCounts.OrderBy(x => x.Value).FirstOrDefault().Key;
    }

    public List<int> GetRoomsWithinDistance(int distance)
    {
        return RememberedRoomsAndRoomLinks
            .Where(x => Extensions.DijkstraGetShortestPathTo(CurrentRoom, x.Key, RememberedRoomsAndRoomLinks, RoomExclusions.ToList()).Count - 1 <= distance)
            .Select(x => x.Key)
            .ToList();
    }

    public List<int> MakePathTowards(int room)
    {
        var currentPath =
            Extensions.DijkstraGetShortestPathTo(CurrentRoom, room, RememberedRoomsAndRoomLinks,
                RoomExclusions.ToList());

        if (currentPath.Count <= 0)
            return new List<int>();

        currentPath.RemoveAt(0); // Remove the current room from the path

        return currentPath;
    }

    public List<int> MakePathTowards(int current, int room, bool excludeDestinationRoom = false)
    {
        var currentPath =
            Extensions.DijkstraGetShortestPathTo(current, room, RememberedRoomsAndRoomLinks, RoomExclusions.ToList());

        if (currentPath.Count <= 0)
            return new List<int>();

        currentPath.RemoveAt(0); // Remove the current room from the path

        if (excludeDestinationRoom)
        {
            currentPath.RemoveAt(currentPath.Count - 1); //remove last room (destination)
        }

        return currentPath;
    }
}
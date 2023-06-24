namespace WumpusLib.Entities.Player.Bot;

public class TrackedEntityInfo
{
    public TrackedEntityInfo(Type type, int amountOfRooms)
    {
        Type = type;
        PossibleRooms = Enumerable.Range(1, amountOfRooms).ToList();
        RoomProbabilities = new Dictionary<int, double>();
        foreach (var room in PossibleRooms) RoomProbabilities[room] = 1.0 / amountOfRooms;
    }

    public TrackedEntityInfo(Type type, List<int> possibleRooms, Dictionary<int, double> probabilities, int amountOfRooms)
    {
        Type = type;
        PossibleRooms = possibleRooms;
        RoomProbabilities = probabilities;
    }

    public Type Type { get; }
    public List<int> PossibleRooms { get; }
    public Dictionary<int, double> RoomProbabilities { get; }
}
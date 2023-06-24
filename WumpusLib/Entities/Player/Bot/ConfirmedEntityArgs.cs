namespace WumpusLib.Entities.Player.Bot;

public class ConfirmedEntityArgs: EventArgs
{
    public ConfirmedEntityArgs(int room, Type type, double probability)
    {
        Room = room;
        Type = type;
        Probability = probability;
    }

    public ConfirmedEntityArgs(int room, Type type)
    {
        Room = room;
        Type = type;
        Probability = 1;
    }

    public Type Type { get; }
    public int Room { get; }

    public double Probability { get; }
}
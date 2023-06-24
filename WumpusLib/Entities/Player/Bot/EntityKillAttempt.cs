namespace WumpusLib.Entities.Player.Bot;

public class EntityKillAttempt
{
    public EntityKillAttempt(Type type, int room)
    {
        Room = room;
        EntityType = type;
    }

    public Type EntityType { get; }
    public int Room { get; }
}
namespace WumpusLib.Entities.Events;

public class EntityDiedArgs
{
    public EntityDiedArgs(Entity died, Entity killed, int room)
    {
        EntityThatDied = died;
        EntityThatKilled = killed;
        Room = room;
    }

    public int Room { get; }
    public Entity EntityThatDied { get; }
    public Entity EntityThatKilled { get; }
}
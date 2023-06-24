namespace WumpusLib.Entities.Events;

public class EntityMovedByEntityArgs : EventArgs
{
    public EntityMovedByEntityArgs(Entity controllingEntity, Entity? entityThatMoved, int originalRoom, int newRoom)
    {
        ControllingEntity = controllingEntity;
        EntityThatMoved = entityThatMoved;
        OriginalRoom = originalRoom;
        NewRoom = newRoom;
    }

    public Entity ControllingEntity { get; }
    public Entity? EntityThatMoved { get; }

    public int OriginalRoom { get; }

    public int NewRoom { get; }
}
using WumpusLib.Entities;

namespace WumpusLib.Map.Events;

public class GameMapUpdateArgs : EventArgs
{
    public GameMapUpdateArgs(Entity entity, int newRoom)
    {
        OriginalRoom = 0;
        NewRoom = newRoom;
        EntityThatMoved = entity;
        ControllingEntity = null;
    }

    public GameMapUpdateArgs(Entity entity, int originalRoom, int newRoom)
    {
        OriginalRoom = originalRoom;
        NewRoom = newRoom;
        EntityThatMoved = entity;
        ControllingEntity = null;
    }

    public GameMapUpdateArgs(Entity? controllingEntity, Entity movingEntity, int originalRoom, int newRoom)
    {
        OriginalRoom = originalRoom;
        NewRoom = newRoom;
        ControllingEntity = controllingEntity;
        EntityThatMoved = movingEntity;
    }

    public int OriginalRoom { get; }
    public int NewRoom { get; }
    public Entity? ControllingEntity { get; }

    public Entity EntityThatMoved { get; }
}
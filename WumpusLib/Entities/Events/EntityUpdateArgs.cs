namespace WumpusLib.Entities.Events;

public class EntityUpdateArgs : EventArgs
{
    public Entity Entity;

    public EntityUpdateArgs(Entity entity)
    {
        Entity = entity;
    }
}
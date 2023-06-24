namespace WumpusLib.Entities.Events;

public class EntityDeployArgs
{
    public EntityDeployArgs(Entity deployingEntity, int newRoom)
    {
        DeployingEntity = deployingEntity;
        Room = newRoom;
    }

    public Entity DeployingEntity { get; }
    public int Room { get; }
}
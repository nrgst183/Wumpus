using WumpusLib.Map;

namespace WumpusLib.Game.Builders;

public class EntitySpawnParametersBuilder
{
    private Type _entityType;
    private int _amount;
    private int _minimumSeparationDistance;
    private List<Type> _incompatibleEntities = new();

    public EntitySpawnParametersBuilder ForEntity(Type entityType)
    {
        _entityType = entityType;
        return this;
    }

    public EntitySpawnParametersBuilder SetAmount(int amount)
    {
        _amount = amount;
        return this;
    }

    public EntitySpawnParametersBuilder SetMinimumSeparationDistance(int minSeparationDistance)
    {
        _minimumSeparationDistance = minSeparationDistance;
        return this;
    }

    public EntitySpawnParametersBuilder AddIncompatibleEntities(params Type[] incompatibleEntities)
    {
        _incompatibleEntities.AddRange(incompatibleEntities);
        return this;
    }

    public void BuildAndAddTo(List<EntitySpawnParameters> targetList)
    {
        var parameters = new EntitySpawnParameters(_entityType, _amount, _minimumSeparationDistance, _incompatibleEntities);
        targetList.Add(parameters);
    }
}
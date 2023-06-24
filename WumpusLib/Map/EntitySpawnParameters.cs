namespace WumpusLib.Map;

public class EntitySpawnParameters
{
    public Type EntityType { get; }
    public int AmountToSpawn { get; }
    public int DegreeOfSpawnSeparation { get; }
    public List<Type> IncompatibleSpawningObjects { get; }

    public EntitySpawnParameters(Type entityType, int amountToSpawn, int degreeOfSpawnSeparation, List<Type> incompatibleSpawningObjects)
    {
        EntityType = entityType;
        AmountToSpawn = amountToSpawn;
        DegreeOfSpawnSeparation = degreeOfSpawnSeparation;
        IncompatibleSpawningObjects = incompatibleSpawningObjects ?? new List<Type>();
    }
}
namespace WumpusLib.Map;

public class NearbyEntitySenseInfo
{
    public NearbyEntitySenseInfo(Type entity, int strength, int distance)
    {
        Entity = entity;
        Strength = strength;
        Distance = distance;
    }

    public Type Entity { get; }

    public int Strength { get; }

    public int Distance { get; }
}
namespace WumpusLib.Game;

public class ScoreRule
{
    public Type EntityType { get; }
    public int ScoreIncrement { get; }
    public string Trigger { get; }

    public ScoreRule(Type entityType, int scoreIncrement, string trigger)
    {
        EntityType = entityType;
        ScoreIncrement = scoreIncrement;
        Trigger = trigger;
    }

    public bool AppliesTo(Type entityType, string trigger)
    {
        return EntityType == entityType && Trigger == trigger;
    }
}
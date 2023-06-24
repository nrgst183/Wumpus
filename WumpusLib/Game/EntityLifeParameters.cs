namespace WumpusLib.Game;

public class EntityLifeParameters
{
    public Type EntityType { get; }
    public int AmountOfLives { get; }
    public bool ImmediatelyEndsGame { get; }
    public bool CanRespawn { get; }
    public int LivesBeforeNoRespawn { get; }

    public EntityLifeParameters(Type entityType, int amountOfLives, bool immediatelyEndsGame, bool canRespawn, int livesBeforeNoRespawn)
    {
        EntityType = entityType;
        AmountOfLives = amountOfLives;
        ImmediatelyEndsGame = immediatelyEndsGame;
        CanRespawn = canRespawn;
        LivesBeforeNoRespawn = livesBeforeNoRespawn;
    }
}
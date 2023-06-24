using WumpusLib.Entities;

namespace WumpusLib.Game;

public class EntityKillInformation
{
    public EntityKillInformation(Entity entityThatKilled, Entity entityThatWasKilled)
    {
        EntityThatWasKilled = entityThatWasKilled;
        EntityThatKilled = entityThatKilled;
    }

    public Entity EntityThatKilled { get; }
    public Entity EntityThatWasKilled { get; }
}
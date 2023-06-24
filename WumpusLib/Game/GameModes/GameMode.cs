using WumpusLib.Entities;
using WumpusLib.Game.Builders;
using WumpusLib.Map;

namespace WumpusLib.Game.GameModes;

public abstract class GameMode
{
    public List<EntityLifeParameters> EntityLifeParameters = new();
    public List<EntitySpawnParameters> EntitySpawnParameters = new();
    public List<GameEndCondition> GameEndConditions { get; } = new();
    public Dictionary<Team, List<Entity>> TeamMapping = new();
    public Dictionary<Type, List<Type>> BottomlessInventoryEntities { get; } = new();
    public GameModeOptions GameModeOptions { get; }

    public abstract int MaximumPlayers { get; }
    public abstract int MinimumPlayers { get; }
    public virtual bool PlayerPvp { get; }
    public virtual bool IsTeamBasedGametype { get; }
    public virtual bool HasBases { get; }

    protected GameMode(GameModeOptions gameModeOptions)
    {
        GameModeOptions = gameModeOptions;

        foreach (var obj in gameModeOptions.EntityLifeSettings)
        {
            EntityLifeParameters.Add(obj);
        }
    }

    public virtual bool IsGameEndCondition(WumpusGame game)
    {
        return GameEndConditions.Any(x => x.Condition.Invoke(game));
    }
    public void AddGameEndCondition(GameEndCondition endCondition)
    {
        GameEndConditions.Add(endCondition);
    }

    public IEnumerable<Entity> SelectGameWinners(WumpusGame game)
    {
        foreach (var endCondition in GameEndConditions.Where(endCondition => endCondition.Condition(game)))
        {
            return endCondition.WinnersSelector(game);
        }

        // Default behavior if no conditions are met
        return Enumerable.Empty<Entity>();
    }
}
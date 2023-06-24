using WumpusLib.Entities;

namespace WumpusLib.Game.GameModes;

public class GameEndCondition
{
    public Func<WumpusGame, bool> Condition { get; }
    public Func<WumpusGame, IEnumerable<Entity>> WinnersSelector { get; }

    public GameEndCondition(Func<WumpusGame, bool> condition, Func<WumpusGame, IEnumerable<Entity>> winnersSelector)
    {
        Condition = condition;
        WinnersSelector = winnersSelector;
    }
}
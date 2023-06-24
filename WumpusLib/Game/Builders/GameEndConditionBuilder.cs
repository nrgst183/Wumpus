namespace WumpusLib.Game.Builders;

public class GameEndConditionBuilder
{
    private readonly List<Func<WumpusGame, bool>> _endConditions;

    public GameEndConditionBuilder()
    {
        _endConditions = new List<Func<WumpusGame, bool>>();
    }

    public GameEndConditionBuilder When(Func<WumpusGame, bool> condition)
    {
        _endConditions.Add(condition);
        return this;
    }

    public List<Func<WumpusGame, bool>> Build()
    {
        return _endConditions;
    }
}
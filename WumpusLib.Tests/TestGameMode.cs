using Moq;
using WumpusLib.Entities;
using WumpusLib.Entities.Player;
using WumpusLib.Game;
using WumpusLib.Game.GameModes;
using WumpusLib.Map;

namespace WumpusLib.Tests;

public class TestGameMode : GameMode
{
    public TestGameMode(GameModeOptions options) : base(options)
    {
        EntitySpawnParameters.Add(new EntitySpawnParameters(typeof(Wumpus), 1, 2, new List<Type> { typeof(Movable) }));
        EntitySpawnParameters.Add(new EntitySpawnParameters(typeof(BottomlessPit), 2, 1, new List<Type> { typeof(Hazard), typeof(Movable) }));
        EntitySpawnParameters.Add(new EntitySpawnParameters(typeof(SuperBats), 2, 2, new List<Type> { typeof(Hazard), typeof(Movable) }));
    }

    public override int MaximumPlayers => 10;
    public override int MinimumPlayers => 1;
    public override bool PlayerPvp => true;
    public override bool IsTeamBasedGametype => false;
    public override bool HasBases => false;

    public override bool IsGameEndCondition(WumpusGame game)
    {
        return false;
    }
}
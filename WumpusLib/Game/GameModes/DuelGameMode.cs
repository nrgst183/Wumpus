using WumpusLib.Entities;
using WumpusLib.Entities.Player;
using WumpusLib.Game.Builders;

namespace WumpusLib.Game.GameModes;

public class DuelGameMode : GameMode
{
    public DuelGameMode(GameModeOptions options, int amountOfRooms = 20) : base(options)
    {
        var amountOfHazardsToSpawn = amountOfRooms / 10;

        new EntitySpawnParametersBuilder()
            .ForEntity(typeof(BottomlessPit))
            .SetAmount(amountOfHazardsToSpawn)
            .SetMinimumSeparationDistance(2)
            .AddIncompatibleEntities(typeof(Hazard), typeof(Movable))
            .BuildAndAddTo(EntitySpawnParameters);

        new EntitySpawnParametersBuilder()
            .ForEntity(typeof(SuperBats))
            .SetAmount(amountOfHazardsToSpawn)
            .SetMinimumSeparationDistance(2)
            .AddIncompatibleEntities(typeof(Hazard), typeof(Movable))
            .BuildAndAddTo(EntitySpawnParameters);

        AddGameEndCondition(new GameEndCondition(
            game => game.EntityKills.Any(kill => kill.EntityThatWasKilled is Player),
            game => game.EntityKills.Select(kill => kill.EntityThatKilled)
        ));
    }

    public override int MaximumPlayers => 2;
    public override int MinimumPlayers => 2;
    public override bool PlayerPvp => true;

    public override bool IsGameEndCondition(WumpusGame map)
    {
        return map.EntityDeathCountMapping.Any();
    }
}
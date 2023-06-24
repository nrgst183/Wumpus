using WumpusLib.Entities;
using WumpusLib.Entities.Player;
using WumpusLib.Game.Builders;

namespace WumpusLib.Game.GameModes;

public class ClassicGameMode : GameMode
{
    public ClassicGameMode(GameModeOptions options, int amountOfRooms = 20) : base(options)
    {
        var amountOfHazardsToSpawn = amountOfRooms / 10;

        new EntitySpawnParametersBuilder()
            .ForEntity(typeof(Wumpus))
            .SetAmount(1)
            .SetMinimumSeparationDistance(1)
            .AddIncompatibleEntities(typeof(Movable))
            .BuildAndAddTo(EntitySpawnParameters);

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

        new EntitySpawnParametersBuilder()
            .ForEntity(typeof(Player))
            .SetAmount(1)
            .SetMinimumSeparationDistance(2)
            .AddIncompatibleEntities(typeof(Hazard), typeof(Movable))
            .BuildAndAddTo(EntitySpawnParameters);

        new EntityLifeParametersBuilder()
            .ForEntity(typeof(Player))
            .SetLives(1)
            .ImmediatelyEndGame()
            .BuildAndAddTo(EntityLifeParameters);

        new EntityLifeParametersBuilder()
            .ForEntity(typeof(Wumpus))
            .SetLives(1)
            .ImmediatelyEndGame()
            .BuildAndAddTo(EntityLifeParameters);

        AddGameEndCondition(new GameEndCondition(
            condition: game => game.Players.Values.Any(player => player.AmountOfArrows == 0),
            winnersSelector: game => game.GameMap.GetEntitiesOnMapByType<Wumpus>(false)
        ));

        AddGameEndCondition(new GameEndCondition(
            condition: game => game.EntityKills.Any(kill => kill.EntityThatWasKilled is Wumpus),
            winnersSelector: game => game.EntityKills.Select(kill => kill.EntityThatKilled)
        ));

        AddGameEndCondition(new GameEndCondition(
            condition: game => game.EntityKills.Any(kill => kill.EntityThatWasKilled is Player),
            winnersSelector: game => game.EntityKills.Select(kill => kill.EntityThatKilled)
        ));
    }

    public override int MaximumPlayers => 1;
    public override int MinimumPlayers => 1;
}
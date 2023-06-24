using WumpusLib.Game.GameModes;

namespace WumpusLib.Entities.Player.Bot;

public class BotPlayer : Player
{
    public BotPlayer(BotSkillLevel skillLevel, string name, List<Arrow> startingArrows,
        EntityGameMapAccessor gameMapAccessor, GameMode gameMode) : base(name,
        startingArrows, gameMapAccessor)
    {
        SkillLevel = skillLevel;

        EntityGameMapAccessor.OnTurnStarted += EntityGameMapAccessor_OnTurnStarted;

        EntityTracker = new EntityTracker(gameMode, gameMapAccessor.AmountOfRooms);
        RoomPathfinder = new RoomPathfinder(CurrentRoom, RoomLinks);

        EntityTracker.OnEntityOfInterestLocationConfirmed += EntityTracker_OnEntityOfInterestLocationConfirmed;

        //We want to know when enemy locations are confirmed, but we'll passively ignore hazards:
        foreach (var type in gameMode.EntitySpawnParameters.Where(x => x.EntityType.IsAssignableFrom(typeof(Enemy))).Select(y => y.EntityType))
        {
            EntityTracker.RegisterEntityTypeOfInterest(type, 0.5f);
        }
    }

    public EntityTracker EntityTracker { get; }
    public RoomPathfinder RoomPathfinder { get; }

    protected Queue<int> CurrentMovementPath { get; } = new();

    public BotSkillLevel SkillLevel { get; }

    private void EntityGameMapAccessor_OnTurnStarted(object? sender, EventArgs e)
    {
        //if we somehow get here and we're dead or can't move, ignore:
        if (IsDead || !EntityGameMapAccessor.GameStarted || !EntityGameMapAccessor.IsPlayersTurn)
            return;

        //update our rooms:
        RoomPathfinder.Update(CurrentRoom, RoomLinks);

        //update the entity tracker:
        EntityTracker.Update(CurrentRoom, RoomLinks, NearbyEntities);
    }

    private void EntityTracker_OnEntityOfInterestLocationConfirmed(object? sender, ConfirmedEntityArgs e)
    {
        CurrentMovementPath.Clear();

        foreach (var room in RoomPathfinder.MakePathTowards(CurrentRoom, e.Room, true))
        {
            CurrentMovementPath.Enqueue(room);
        }
    }
}
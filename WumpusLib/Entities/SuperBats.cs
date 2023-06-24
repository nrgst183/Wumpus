using WumpusLib.Entities.Events;

namespace WumpusLib.Entities;

public class SuperBats : Hazard
{
    private readonly int _amountOfRooms;

    public SuperBats(int amountOfRooms, EntityGameMapAccessor gameMapAccessor)
        : base(gameMapAccessor)
    {
        _amountOfRooms = amountOfRooms;
        EntityGameMapAccessor.OnEntityEnterCurrentRoom += SuperBats_OnEntityEnterCurrentRoom;
    }

    public sealed override bool CanAttackOtherEntities => false;

    public override int DamageDealt => 0;
    public override bool CanMoveOtherEntities => true;
    public override bool CanSeeAllRooms => true;

    private void SuperBats_OnEntityEnterCurrentRoom(object? sender, EntityUpdateArgs e)
    {
        if (e.Entity is not Wumpus && !e.Entity.IsEquippable && e.Entity is not Hazard)
            EntityGameMapAccessor.MoveEntityTo(e.Entity, GetRandomRoom());
    }

    private int GetRandomRoom()
    {
        int randNumber;

        do
        {
            randNumber = Extensions.Random.Next(1, _amountOfRooms);
        } while (randNumber == CurrentRoom);

        return randNumber;
    }
}
using WumpusLib.Entities.Events;
using WumpusLib.Game;

namespace WumpusLib.Entities;

public class Wumpus : Enemy
{
    public Wumpus(EntityGameMapAccessor gameMapAccessor)
        : base(gameMapAccessor)
    {
        HostileEntities.Add(typeof(Player.Player));

        EntityGameMapAccessor.OnEntityLeaveNearbyRoom += Wumpus_OnEntityLeaveNearbyRoom;
        EntityGameMapAccessor.OnEntityEnterCurrentRoom += Wumpus_OnEntityEnterCurrentRoom;
        EntityGameMapAccessor.OnMove += Wumpus_OnMove;
    }

    public override int DamageDealt => GlobalEntityAttributes.PlayerHealth;

    public override bool CanMoveOtherEntities => false;
    public override int EntitySenseBroadcastSize => 1;

    private void Wumpus_OnMove(object? sender, EntityMovedArgs e)
    {
        foreach (var entity in EntitiesInRoom.Where(IsEntityAttackable)) InflictDamage(entity);
    }

    private void Wumpus_OnEntityEnterCurrentRoom(object? sender, EntityUpdateArgs e)
    {
        if (IsEntityAttackable(e.Entity)) InflictDamage(e.Entity);
    }

    private void Wumpus_OnEntityLeaveNearbyRoom(object? sender, EntityUpdateArgs e)
    {
        //adds mechanic where wumpus moves to random link if arrow missed him:
        if (e.Entity is not Arrow)
            return;

        var randomIndex = Extensions.Random.Next(RoomLinks.Count);

        EntityGameMapAccessor.MoveTo(RoomLinks[randomIndex]);
    }

    private bool IsEntityAttackable(Entity entity)
    {
        return HostileEntities.Contains(entity.GetType());
    }
}
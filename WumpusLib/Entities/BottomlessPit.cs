using WumpusLib.Entities.Events;
using WumpusLib.Game;

namespace WumpusLib.Entities;

public class BottomlessPit : Hazard
{
    public BottomlessPit(EntityGameMapAccessor gameMapAccessor)
        : base(gameMapAccessor)
    {
        HostileEntities.Add(typeof(Player.Player));
        EntityGameMapAccessor.OnEntityEnterCurrentRoom += BottomlessPit_OnEntityEnterCurrentRoom;
    }


    public override int DamageDealt => GlobalEntityAttributes.PlayerHealth;
    public override bool CanAttackOtherEntities => true;

    private void BottomlessPit_OnEntityEnterCurrentRoom(object? sender, EntityUpdateArgs e)
    {
        if (HostileEntities.Contains(e.Entity.GetType()))
            InflictDamage(e.Entity);
    }
}
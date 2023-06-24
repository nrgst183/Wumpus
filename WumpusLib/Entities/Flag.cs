using WumpusLib.Game.GameModes;

namespace WumpusLib.Entities;

public class Flag : Entity
{
    public Flag(Team teamAllegiance, EntityGameMapAccessor gameMapAccessor) : base(gameMapAccessor)
    {
        TeamAllegiance = teamAllegiance;
    }

    public override int EntitySenseBroadcastSize => 5;

    public Team TeamAllegiance { get; }
    public override bool IsEquippable => true;
    public sealed override bool IsWeapon => false;
    public sealed override bool CanMove => false;
    public sealed override bool CanMoveToAnyRoom => false;
    public sealed override bool CanReceiveDamage => false;
}
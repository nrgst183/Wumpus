namespace WumpusLib.Entities;

public abstract class Hazard : Entity
{
    protected Hazard(EntityGameMapAccessor gameMapAccessor)
        : base(gameMapAccessor)
    {
    }

    public sealed override bool IsEquippable => false;
    public sealed override bool IsWeapon => false;
    public sealed override bool CanMove => false;
    public sealed override bool CanMoveToAnyRoom => false;
    public sealed override bool CanReceiveDamage => false;
}
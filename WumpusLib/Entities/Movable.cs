namespace WumpusLib.Entities;

public abstract class Movable : Entity
{
    protected Movable(EntityGameMapAccessor gameMapAccessor) : base(gameMapAccessor)
    {
    }

    public override bool IsEquippable => false;
    public override bool IsWeapon => false;
    public sealed override bool CanMove => true;
    public override bool CanMoveToAnyRoom => false;
    public override bool CanReceiveDamage => true;
}
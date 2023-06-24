namespace WumpusLib.Entities;

public class Enemy : Movable
{
    public Enemy(EntityGameMapAccessor gameMapAccessor)
        : base(gameMapAccessor)
    {
    }

    public sealed override bool CanAttackOtherEntities => true;
    public sealed override bool CanReceiveDamage => true;

    public override int Health { get; protected set; } = 10;
}
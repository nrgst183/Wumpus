using WumpusLib.Entities.Events;
using WumpusLib.Game;

namespace WumpusLib.Entities;

public class Arrow : Movable
{
    private DateTime _lastFired;

    private int _timesFiredCounter;

    public Arrow(EntityGameMapAccessor gameMapAccessor) : base(gameMapAccessor)
    {
        gameMapAccessor.OnMovedByOtherEntity += GameMapAccessorOnMovedByOtherEntity;
        gameMapAccessor.OnDeployedToMap += GameMapAccessorOnDeployedToMap;
        _timesFiredCounter = 0;
    }

    public override int DamageDealt => GlobalEntityAttributes.ArrowDamageDealt;
    public override bool CanAttackOtherEntities => IsEquippable;
    public override bool CanReceiveDamage => false;
    public override bool IsWeapon => true;
    public override int Health { get; protected set; } = 10;
    public override int EntitySenseBroadcastSize => 0;

    public override bool IsEquippable => _timesFiredCounter <= 1;

    public string OwnerName { get; private set; }

    public string PreviousOwnerName =>
        Extensions.GetObscuredStringByTimeElapsed(UnObscuredPreviousOwnerName, _lastFired, 15, '_');

    private string UnObscuredPreviousOwnerName { get; set; }

    private void GameMapAccessorOnDeployedToMap(object? sender, EntityDeployArgs e)
    {
        //we're about to be fired, mark the player name down:
        if (e.DeployingEntity is Player.Player player) OwnerName = player.PlayerName;
    }

    private void GameMapAccessorOnMovedByOtherEntity(object? sender, EntityMovedByEntityArgs e)
    {
        //get first enemy that can be attacked
        var entitiesToAttack =
            EntitiesInRoom.Where(x => x.CanReceiveDamage && x is not Arrow).OrderByDescending(x => x is Enemy).ToList();

        //"hit" first enemy with arrow and deactivate
        if (entitiesToAttack.Any())
        {
            InflictDamage(entitiesToAttack.First());
            Health = 0;
        }
        else
        {
            //land in cave without hitting anyone

            if (_timesFiredCounter >= 1)
                return;

            _lastFired = DateTime.Now;

            _timesFiredCounter++;

            UnObscuredPreviousOwnerName = OwnerName;

            OwnerName = "";
        }
    }
}
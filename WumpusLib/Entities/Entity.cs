using WumpusLib.Entities.Events;
using WumpusLib.Map;

namespace WumpusLib.Entities;

public abstract class Entity
{
    public readonly List<Type> HostileEntities = new();

    protected readonly Inventory Inventory = new(3);

    public Entity? EntityThatDeployedThis;

    protected Entity(EntityGameMapAccessor gameMapAccessor)
    {
        EntityGameMapAccessor = gameMapAccessor;
        gameMapAccessor.LinkEntityToAccessor(this);
        gameMapAccessor.OnDeployedToMap += GameMapAccessorOnDeployedToMap;
        gameMapAccessor.OnRemovedFromMap += GameMapAccessorOnRemovedFromMap;
    }

    public int CurrentRoom => EntityGameMapAccessor.CurrentRoom;
    protected List<int> RoomLinks => EntityGameMapAccessor.RoomLinks;
    protected List<Entity> EntitiesInRoom => EntityGameMapAccessor.EntitiesInRoom;
    public List<NearbyEntitySenseInfo> NearbyEntities => EntityGameMapAccessor.NearbyEntities;

    protected EntityGameMapAccessor EntityGameMapAccessor { get; }

    private int _health;
    private int _previousHealth;
    public virtual int Health
    {
        get => _health;
        protected set
        {
            _previousHealth = _health;
            _health = value;
        }
    }

    public abstract bool IsEquippable { get; }
    public abstract bool IsWeapon { get; }
    public abstract bool CanMove { get; }
    public abstract bool CanMoveToAnyRoom { get; }
    public virtual bool CanAttackOtherEntities => false;
    public abstract bool CanReceiveDamage { get; }
    public virtual bool CanMoveOtherEntities => false;
    public virtual bool CanSeeAllRooms => false;

    public virtual bool CanMoveEntitiesToAnyRoom => false;

    public virtual int DamageDealt => 10;
    public virtual int EntitySenseBroadcastSize => 1;

    public bool IsDead => Health <= 0;


    public event EventHandler<EntityDiedArgs>? OnKilled;

    public int ReceiveDamage(Entity entity, int damage)
    {
        if (!CanReceiveDamage)
            return 0;

        if (damage <= 0)
            return 0;

        Health -= damage;

        if (IsDead && _previousHealth > 0)
            HandleOnKilledEvent(entity);

        return damage;
    }

    public int InflictDamage(Entity entity)
    {
        return CanAttackOtherEntities && HostileEntities.Contains(entity.GetType())
            ? entity.ReceiveDamage(this, DamageDealt)
            : 0;
    }

    private void HandleOnKilledEvent(Entity entityThatKilledUs)
    {
        //if entity that killed us was a weapon (arrow) or equippable (also arrow), inform gamemap of actual assassin
        if (entityThatKilledUs.IsWeapon ||
            (entityThatKilledUs.IsEquippable && entityThatKilledUs.EntityThatDeployedThis != null))
            OnKilled?.Invoke(this, new EntityDiedArgs(this, entityThatKilledUs.EntityThatDeployedThis, CurrentRoom));
        else
            OnKilled?.Invoke(this, new EntityDiedArgs(this, entityThatKilledUs, CurrentRoom));
    }

    private void GameMapAccessorOnDeployedToMap(object? sender, EntityDeployArgs e)
    {
        if (IsEquippable || (IsWeapon && !IsDead)) EntityThatDeployedThis = e.DeployingEntity;
    }

    private void GameMapAccessorOnRemovedFromMap(object? sender, EntityUpdateArgs e)
    {
        if (IsEquippable || (IsWeapon && !IsDead)) EntityThatDeployedThis = null;
    }
}
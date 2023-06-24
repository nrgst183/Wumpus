using WumpusLib.Entities;

namespace WumpusLib.Tests;

public class TestEntity : Entity
{
    public TestEntity(EntityGameMapAccessor gameMapAccessor) : base(gameMapAccessor)
    {
    }

    public override int EntitySenseBroadcastSize => EntitySenseBroadcastSizeOverride;
    public override bool IsWeapon => false;
    public override bool CanMove => CanMoveOverride;
    public override bool CanMoveToAnyRoom => CanMoveToAnyRoomOverride;
    public override bool CanReceiveDamage => CanReceiveDamageOverride;
    public override bool IsEquippable => IsEquippableOverride;

    public override bool CanSeeAllRooms => CanSeeAllRoomsOverride;


    public int CustomHealth { get; set; } = 10;

    public bool CanMoveOverride { get; set; } = true;

    public bool CanMoveToAnyRoomOverride { get; set; } = true;
    public bool CanReceiveDamageOverride { get; set; } = true;
    public bool IsEquippableOverride { get; set; } = true;
    public bool CanSeeAllRoomsOverride { get; set; } = true;
    public bool CanMoveOtherEntitiesOverride { get; set; } = true;
    public int EntitySenseBroadcastSizeOverride { get; set; } = 1;


    public override bool CanMoveOtherEntities => CanMoveOtherEntitiesOverride;

    public override int Health
    {
        get => CustomHealth;
        protected set => CustomHealth = value;
    }
}
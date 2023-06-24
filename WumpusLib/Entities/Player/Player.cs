using WumpusLib.Map.Results;

namespace WumpusLib.Entities.Player;

public class Player : Movable
{
    public Player(string name, List<Arrow> arrows, EntityGameMapAccessor gameMapAccessor)
        : base(gameMapAccessor)
    {
        PlayerName = name;

        foreach (var arrow in arrows) Inventory.Add(arrow);
    }

    public string PlayerName { get; }

    public int AmountOfArrows => Inventory.Count<Arrow>();

    public override bool CanAttackOtherEntities => false;
    public override int DamageDealt => 0;
    public override int Health { get; protected set; } = 10;

    public MoveResult Shoot(int room)
    {
        if (AmountOfArrows <= 0)
            return MoveResult.IllegalMove;

        var arrow = (Arrow) Inventory.Remove<Arrow>();

        var moveResult = EntityGameMapAccessor.DeployEquippableEntity(arrow);

        return moveResult == MoveResult.Success ? EntityGameMapAccessor.MoveEntityTo(arrow, room) : moveResult;
    }

    public MoveResult Move(int room)
    {
        return EntityGameMapAccessor.MoveTo(room);
    }

    protected EquipResult AddEntityToInventoryFromCurrentRoom(Entity entity)
    {
        var removeResult = EntityGameMapAccessor.RemoveEquippableEntity(entity);

        return removeResult == EquipResult.Success ? Inventory.Add(entity) : removeResult;
    }

    protected EquipResult AddEntityToInventory(Entity entity)
    {
        if (!entity.IsEquippable || !entity.IsDead) return EquipResult.EntityNotEquippable;

        return Inventory.Add(entity);
    }
}
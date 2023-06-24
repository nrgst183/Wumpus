using WumpusLib.Map.Results;

namespace WumpusLib.Entities;

public class Inventory
{
    private readonly List<Entity> _items = new();

    public Inventory(int maxAmount)
    {
        MaxAmount = maxAmount;
    }

    public int MaxAmount { get; }

    public int TotalCount => _items.Count;

    public EquipResult Add(Entity item)
    {
        if (!item.IsEquippable) return EquipResult.EntityNotEquippable;

        if (_items.Count >= MaxAmount) return EquipResult.InventoryFull;

        _items.Add(item);

        return EquipResult.Success;
    }

    public Entity Remove<T>()
    {
        var ent = _items.FirstOrDefault(item => item.GetType().IsAssignableFrom(typeof(T)));
        _items.Remove(ent);
        return ent;
    }

    public int Count<T>()
    {
        return _items.Count(item => item.GetType().IsAssignableFrom(typeof(T)));
    }

    public void Clear()
    {
        _items.Clear();
    }
}
namespace WumpusLib.Map.Results;

public enum EquipResult
{
    Success,
    EntityNotEquippable,
    EntityNotOnMap,
    EntityNotInCurrentRoom,
    InventoryFull,
    ItemOfTypeNotInInventory
}
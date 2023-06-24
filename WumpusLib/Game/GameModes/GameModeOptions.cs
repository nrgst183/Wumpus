using WumpusLib.Map;

namespace WumpusLib.Game.GameModes;

public class GameModeOptions
{
    public bool IsAmountOfLivesBasedGameType { get; set; } = true;
    public bool IsTimeBasedGameType { get; set; } = false;
    public TimeSpan TotalTimeLimit { get; set; } = TimeSpan.FromMinutes(5);

    public List<EntityLifeParameters> EntityLifeSettings = new();

    public Dictionary<Type, List<Type>> BottomlessInventoryEntities { get; } = new();

    public int AmountOfStartingArrows { get; set; } = 3;

    public bool IsNotTurnBased { get; set; } = false;

    public bool PlayerPVP { get; set; } = true;
}
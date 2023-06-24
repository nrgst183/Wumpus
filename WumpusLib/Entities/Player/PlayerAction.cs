namespace WumpusLib.Entities.Player;

public class PlayerAction
{
    public PlayerAction(PlayerActionType action, int room)
    {
        Action = action;
        Room = room;
    }

    public PlayerActionType Action { get; }
    public int Room { get; }
}
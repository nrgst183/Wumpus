namespace WumpusLib.Entities.Events;

public class EntityTurnArgs : EventArgs
{
    public int Turn;

    public EntityTurnArgs(int turn)
    {
        Turn = turn;
    }
}
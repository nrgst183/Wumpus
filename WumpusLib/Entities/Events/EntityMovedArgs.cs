namespace WumpusLib.Entities.Events;

public class EntityMovedArgs : EventArgs
{
    public EntityMovedArgs(int originalRoom, int newRoom)
    {
        OriginalRoom = originalRoom;
        NewRoom = newRoom;
    }

    public int OriginalRoom { get; }
    public int NewRoom { get; }
}
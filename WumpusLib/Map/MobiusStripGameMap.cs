namespace WumpusLib.Map;

public class MobiusStripGameMap : GameMap
{
    public MobiusStripGameMap(int amountOfRooms)
        : base((amountOfRooms + 1) / 2 * 2) // Round up to the nearest even number
    {
    }

    protected override void BuildMapLayout()
    {
        for (int even = 2, odd = 1; even < AmountOfRooms && odd < AmountOfRooms - 1; even += 2, odd += 2)
        {
            LinkRooms(even, even + 2);
            LinkRooms(odd, odd + 2);
        }

        LinkRooms(1, AmountOfRooms);
        LinkRooms(2, AmountOfRooms - 1);
    }
}
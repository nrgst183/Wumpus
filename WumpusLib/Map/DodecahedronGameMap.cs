namespace WumpusLib.Map;

public class DodecahedronGameMap : GameMap
{
    public DodecahedronGameMap(int amountOfRooms) : base(amountOfRooms)
    {
    }

    public static int DefaultRoomSize => 20;

    public static int MinimumAmountOfTunnels => 2;

    public int AmountOfTunnels =>
        Math.Max((2 * AmountOfRooms + DefaultRoomSize) / DefaultRoomSize, MinimumAmountOfTunnels);

    protected override void BuildMapLayout()
    {
        // Calculate delta: a random number used to generate the initial room connections
        // The Greatest Common Divisor (GCD) between the AmountOfRooms and delta + 1 must be 1
        int delta;
        do
        {
            delta = Extensions.Random.Next(AmountOfRooms - 1) + 1;
        } while (Extensions.GreatestCommonDivisor(AmountOfRooms, delta + 1) != 1);

        // Generate initial random room connections using the calculated delta
        for (var room = 1; room <= AmountOfRooms; room++)
        {
            var lnk = (room + delta) % AmountOfRooms + 1;
            LinkRooms(room, lnk);
        }

        for (var room = 1; room <= AmountOfRooms; room++)
        {
            var amountToAdd = AmountOfTunnels - RoomLinks[room].Count;

            if (amountToAdd <= 0)
                continue;

            for (var i = 0; i < amountToAdd; i++)
            {
                var linkableRoom = GetRoomWithMinimumLinks(room);

                if (linkableRoom != -1) LinkRooms(room, linkableRoom);
            }
        }
    }

    private int GetRoomWithMinimumLinks(int room)
    {
        var minLinks = int.MaxValue;
        var minLinkedRoom = -1;

        for (var r = 1; r <= AmountOfRooms; r++)
        {
            if (r == room || RoomLinks[room].Contains(r) || RoomLinks[r].Count >= AmountOfTunnels)
                continue;

            if (RoomLinks[r].Count >= minLinks)
                continue;

            minLinks = RoomLinks[r].Count;
            minLinkedRoom = r;
        }

        return minLinkedRoom;
    }
}
using WumpusLib.Map;
using WumpusLib.Map.Results;

namespace WumpusLib.Tests;

public class TestGameMap : Map.GameMap
{
    public TestGameMap(int amountOfRooms = 5) : base(amountOfRooms)
    {
    }

    public List<EntitySpawnParameters> TestEntitySpawnParameters => EntitySpawnParameters;

    public LinkResult LinkRoomsPublic(int room1, int room2)
    {
        return LinkRooms(room1, room2);
    }

    public void BuildGameMapPublic()
    {
        BuildMapLayout();
    }

    protected override void BuildMapLayout()
    {
        for (int first = 1, second = 2; first < AmountOfRooms; first++) LinkRooms(first, second);
    }
}
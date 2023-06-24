using WumpusLib.Entities;

namespace WumpusLib.Tests.GameMap;

[TestClass]
public class GetRoomLinksForEntityUnitTests
{
    [TestMethod]
    public void GetRoomLinksForEntity_ReturnsRoomLinks_WhenEntityOnMap()
    {
        // Arrange
        var map = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(map);
        var testEntity = new TestEntity(gameMapAccessor);
        map.AddEntityToRandomRoom(testEntity, 1);
        var room = map.GetRoomNumberForEntity(testEntity);
        Assert.AreNotEqual(0, room, "Entity not placed on map");

        // Act
        var roomLinks = map.GetRoomLinksForEntity(testEntity);

        // Assert
        Assert.IsTrue(roomLinks.SequenceEqual(map.RoomLinks[room]), "Room links should match for the entity's room");
    }

    [TestMethod]
    public void GetRoomLinksForEntity_ReturnsEmptyList_WhenEntityNotOnMap()
    {
        // Arrange
        var map = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(map);
        var testEntity = new TestEntity(gameMapAccessor);

        // Act
        var roomLinks = map.GetRoomLinksForEntity(testEntity);

        // Assert
        Assert.IsTrue(roomLinks.SequenceEqual(new List<int>()), "Room links should be empty for entity not on the map");
    }
}
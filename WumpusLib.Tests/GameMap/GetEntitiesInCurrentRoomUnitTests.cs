using WumpusLib.Entities;

namespace WumpusLib.Tests.GameMap;

[TestClass]
public class GetEntitiesInCurrentRoomUnitTests
{
    [TestMethod]
    public void GetEntitiesInCurrentRoom_ReturnsEntities_WhenEntityOnMap()
    {
        // Arrange
        var map = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(map);
        var testEntity1 = new TestEntity(gameMapAccessor);
        var testEntity2 = new TestEntity(gameMapAccessor);

        map.AddEntityToRandomRoom(testEntity1, 1);
        var room1 = map.GetRoomNumberForEntity(testEntity1);

        Assert.AreNotEqual(0, room1, "Entity 1 not placed on map");

        map.AddEntityToMap(testEntity2, room1);
        var room2 = map.GetRoomNumberForEntity(testEntity2);
        Assert.AreEqual(room1, room2, "Entity 2 should be in the same room as Entity 1");

        // Act
        var entitiesInRoom = map.GetEntitiesInCurrentRoom(testEntity1);

        // Assert
        Assert.IsTrue(entitiesInRoom.Contains(testEntity1), "Entities in room should contain testEntity1");
        Assert.IsTrue(entitiesInRoom.Contains(testEntity2), "Entities in room should contain testEntity2");
    }

    [TestMethod]
    public void GetEntitiesInCurrentRoom_ReturnsEmptyList_WhenEntityNotOnMap()
    {
        // Arrange
        var map = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(map);
        var testEntity = new TestEntity(gameMapAccessor);

        // Act
        var entitiesInRoom = map.GetEntitiesInCurrentRoom(testEntity);

        // Assert
        Assert.IsTrue(entitiesInRoom.SequenceEqual(new List<Entity>()),
            "Entities list should be empty for entity not on the map");
    }
}
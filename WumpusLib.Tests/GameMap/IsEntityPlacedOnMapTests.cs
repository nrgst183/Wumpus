using WumpusLib.Entities;

namespace WumpusLib.Tests.GameMap;

[TestClass]
public class IsEntityPlacedOnMapTests
{
    [TestMethod]
    public void IsEntityPlacedOnMap_EntityNotPlacedOnMap_ReturnsFalse()
    {
        // Arrange
        var map = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(map);
        var entity = new TestEntity(gameMapAccessor);

        // Act
        var isPlaced = map.IsEntityPlacedOnMap(entity);

        // Assert
        Assert.IsFalse(isPlaced, "Expected false when the entity is not placed on the map.");
    }

    [TestMethod]
    public void IsEntityPlacedOnMap_EntityPlacedOnMap_ReturnsTrue()
    {
        // Arrange
        var map = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(map);
        var entity = new TestEntity(gameMapAccessor);
        map.AddEntityToMap(entity, 1);

        // Act
        var isPlaced = map.IsEntityPlacedOnMap(entity);

        // Assert
        Assert.IsTrue(isPlaced, "Expected true when the entity is placed on the map.");
    }
}
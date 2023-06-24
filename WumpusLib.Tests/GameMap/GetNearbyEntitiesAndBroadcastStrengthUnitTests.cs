using WumpusLib.Entities;

namespace WumpusLib.Tests.GameMap;

[TestClass]
public class GetNearbyEntitiesAndBroadcastStrengthUnitTests
{
    [TestMethod]
    public void GetNearbyEntitiesAndBroadcastStrength_EntitiesOutOfRange_ReturnsEmptyList()
    {
        var map = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(map);
        map.BuildGameMapPublic();
        var entity1 = new TestEntity(gameMapAccessor)
        {
            EntitySenseBroadcastSizeOverride = 1
        };

        var entity2 = new TestEntity(gameMapAccessor)
        {
            EntitySenseBroadcastSizeOverride = 1
        };

        map.AddEntityToMap(entity1, 1);
        map.AddEntityToMap(entity2, 3);

        var result = map.GetNearbyEntitiesAndBroadcastStrength(1);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetNearbyEntitiesAndBroadcastStrength_EntitiesWithMixedBroadcastRange_ReturnsFilteredEntities()
    {
        var map = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(map);
        map.BuildGameMapPublic();
        var entity1 = new TestEntity(gameMapAccessor)
        {
            EntitySenseBroadcastSizeOverride = 2
        };

        var entity2 = new TestEntity(gameMapAccessor)
        {
            EntitySenseBroadcastSizeOverride = 1
        };

        map.AddEntityToMap(entity1, 1);
        map.AddEntityToMap(entity2, 2);

        var result = map.GetNearbyEntitiesAndBroadcastStrength(1);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(entity1.GetType(), result[0].Entity);
    }

    [TestMethod]
    public void GetNearbyEntitiesAndBroadcastStrength_NoEntities_ReturnsEmptyList()
    {
        var map = new TestGameMap();

        var result = map.GetNearbyEntitiesAndBroadcastStrength(0);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetNearbyEntitiesAndBroadcastStrength_EntitiesWithinBroadcastRange_ReturnsEntities()
    {
        // Arrange
        var map = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(map);
        map.BuildGameMapPublic();

        var entity1 = new TestEntity(gameMapAccessor)
        {
            EntitySenseBroadcastSizeOverride = 2
        };

        var entity2 = new TestEntity(gameMapAccessor)
        {
            EntitySenseBroadcastSizeOverride = 1
        };

        var entity3 = new TestEntity(gameMapAccessor)
        {
            EntitySenseBroadcastSizeOverride = 1
        };

        map.AddEntityToMap(entity1, 1); // Entity1 should be sensed from room 3 (2 rooms away)
        map.AddEntityToMap(entity2, 2); // Entity2 should be sensed from room 3 (1 room away)
        map.AddEntityToMap(entity3, 4); // Entity3 should not be sensed from room 3 (2 rooms away, but has range 1)

        // Act
        var result = map.GetNearbyEntitiesAndBroadcastStrength(3); // From room 3, only entity1 and entity2 should be detected

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Any(r => r.Entity == typeof(TestEntity) && r.Strength == 2)); // Checking for entity1
        Assert.IsTrue(result.Any(r => r.Entity == typeof(TestEntity) && r.Strength == 1)); // Checking for entity2
    }

}
using WumpusLib.Entities;
using WumpusLib.Map.Results;

namespace WumpusLib.Tests.GameMap;

[TestClass]
public class RemoveEntityUnitTests
{
    [TestMethod]
    public void RemoveEntity_ValidEntities_Success()
    {
        // Arrange
        var testGameMap = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(testGameMap);
        var primaryEntity = new TestEntity(gameMapAccessor)
        {
            CanSeeAllRoomsOverride = true
        };
        var equippableEntity = new TestEntity(gameMapAccessor)
        {
            IsEquippableOverride = true
        };

        testGameMap.AddEntityToMap(primaryEntity, 1);
        testGameMap.AddEntityToMap(equippableEntity, 1);

        // Act
        var result = testGameMap.RemoveEntity(primaryEntity, equippableEntity);

        // Assert
        Assert.AreEqual(EquipResult.Success, result);
    }

    [TestMethod]
    public void RemoveEntity_EntityNotInCurrentRoom_ReturnsEntityNotInCurrentRoom()
    {
        // Arrange
        var testGameMap = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(testGameMap);
        var primaryEntity = new TestEntity(gameMapAccessor)
        {
            CanSeeAllRoomsOverride = false
        };

        var equippableEntity = new TestEntity(gameMapAccessor)
        {
            IsEquippableOverride = true
        };

        testGameMap.AddEntityToMap(primaryEntity, 1);
        testGameMap.AddEntityToMap(equippableEntity, 2);

        // Act
        var result = testGameMap.RemoveEntity(primaryEntity, equippableEntity);

        // Assert
        Assert.AreEqual(EquipResult.EntityNotInCurrentRoom, result);
    }

    [TestMethod]
    public void RemoveEntity_EntityNotEquippable_ReturnsEntityNotEquippable()
    {
        // Arrange
        var testGameMap = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(testGameMap);
        var primaryEntity = new TestEntity(gameMapAccessor);
        var nonEquippableEntity = new TestEntity(gameMapAccessor)
        {
            IsEquippableOverride = false
        };

        testGameMap.AddEntityToMap(primaryEntity, 1);
        testGameMap.AddEntityToMap(nonEquippableEntity, 1);

        // Act
        var result = testGameMap.RemoveEntity(primaryEntity, nonEquippableEntity);

        // Assert
        Assert.AreEqual(EquipResult.EntityNotEquippable, result);
    }


    [TestMethod]
    public void RemoveEntity_EntityNotOnMap_ReturnsEntityNotOnMap()
    {
        var map = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(map);
        var primaryEntity = new TestEntity(gameMapAccessor);
        var entityToEquip = new TestEntity(gameMapAccessor)
        {
            IsEquippableOverride = true
        };

        map.AddEntityToMap(primaryEntity, 1);

        var result = map.RemoveEntity(primaryEntity, entityToEquip);
        Assert.AreEqual(EquipResult.EntityNotOnMap, result);
    }


    [TestMethod]
    public void RemoveEntity_SuccessfulRemoval_ReturnsSuccess()
    {
        var map = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(map);
        var primaryEntity = new TestEntity(gameMapAccessor);
        var entityToEquip = new TestEntity(gameMapAccessor)
        {
            IsEquippableOverride = true
        };

        map.AddEntityToMap(primaryEntity, 1);
        map.AddEntityToMap(entityToEquip, 1);

        var result = map.RemoveEntity(primaryEntity, entityToEquip);
        Assert.AreEqual(EquipResult.Success, result);
    }


    [TestMethod]
    public void RemoveEntity_NullPrimaryEntity_ThrowsArgumentNullException()
    {
        // Arrange
        var testGameMap = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(testGameMap);

        var equippableEntity = new TestEntity(gameMapAccessor)
        {
            IsEquippableOverride = true
        };

        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => testGameMap.RemoveEntity(null, equippableEntity));
    }

    [TestMethod]
    public void RemoveEntity_NullEntityToEquip_ThrowsArgumentNullException()
    {
        // Arrange
        var testGameMap = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(testGameMap);
        var primaryEntity = new TestEntity(gameMapAccessor);

        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => testGameMap.RemoveEntity(primaryEntity, null));
    }
}
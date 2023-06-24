using WumpusLib.Entities;
using WumpusLib.Map.Results;

namespace WumpusLib.Tests.GameMap;

[TestClass]
public class DeployEntityUnitTests
{
    [TestMethod]
    public void DeployEntity_NullPrimaryEntity_ThrowsArgumentNullException()
    {
        var map = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(map);
        var equipableEntity = new TestEntity(gameMapAccessor)
        {
            IsEquippableOverride = true
        };

        Assert.ThrowsException<ArgumentNullException>(() => map.DeployEntity(null, equipableEntity));
    }

    [TestMethod]
    public void DeployEntity_NullEquipableEntity_ThrowsArgumentNullException()
    {
        var map = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(map);
        var primaryEntity = new TestEntity(gameMapAccessor);

        Assert.ThrowsException<ArgumentNullException>(() => map.DeployEntity(primaryEntity, null));
    }

    [TestMethod]
    public void DeployEntity_EntityAlreadyOnMap_ReturnsCannotMoveToSameRoom()
    {
        var map = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(map);
        var primaryEntity = new TestEntity(gameMapAccessor);
        var equipableEntity = new TestEntity(gameMapAccessor)
        {
            IsEquippableOverride = true
        };

        map.AddEntityToMap(primaryEntity, 1);
        map.AddEntityToMap(equipableEntity, 1);

        var result = map.DeployEntity(primaryEntity, equipableEntity);
        Assert.AreEqual(MoveResult.CannotMoveToSameRoom, result);
    }

    [TestMethod]
    public void DeployEntity_SuccessfulDeployment_ReturnsSuccess()
    {
        var map = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(map);
        var primaryEntity = new TestEntity(gameMapAccessor);
        var equipableEntity = new TestEntity(gameMapAccessor)
        {
            IsEquippableOverride = true
        };

        map.AddEntityToMap(primaryEntity, 1);

        var result = map.DeployEntity(primaryEntity, equipableEntity);
        Assert.AreEqual(MoveResult.Success, result);
    }
}
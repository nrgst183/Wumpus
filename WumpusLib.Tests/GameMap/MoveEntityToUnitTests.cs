using WumpusLib.Entities;
using WumpusLib.Map.Results;

namespace WumpusLib.Tests.GameMap;

[TestClass]
public class MoveEntityToUnitTests
{
    [TestMethod]
    public void MoveEntityTo_NullPrimaryEntity_ThrowsArgumentNullException()
    {
        var map = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(map);
        var entity = new TestEntity(gameMapAccessor);

        Assert.ThrowsException<ArgumentNullException>(() => map.MoveEntityTo(null, entity, 1));
    }

    [TestMethod]
    public void MoveEntityTo_NullEntityToMove_ThrowsArgumentNullException()
    {
        var map = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(map);
        var primaryEntity = new TestEntity(gameMapAccessor);

        Assert.ThrowsException<ArgumentNullException>(() => map.MoveEntityTo(primaryEntity, null, 1));
    }

    [TestMethod]
    public void MoveEntityTo_EntityNotAllowedToMove_ReturnsNotAllowedToMove()
    {
        // Arrange
        var map = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(map);
        var primaryEntity = new TestEntity(gameMapAccessor);
        var entityToMove = new TestEntity(gameMapAccessor)
        {
            CanMoveOverride = false
        };

        map.AddEntityToMap(primaryEntity, 1);
        map.AddEntityToMap(entityToMove, 1);

        // Act
        var result = map.MoveEntityTo(primaryEntity, entityToMove, 2);

        // Assert
        Assert.AreEqual(MoveResult.EntityNotAllowedToMove, result, "Expected MoveResult.EntityNotAllowedToMove when entity is not allowed to move.");
    }

    [TestMethod]
    public void MoveEntityTo_IllegalMove_ReturnsIllegalMove()
    {
        // Arrange
        var map = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(map);
        var primaryEntity = new TestEntity(gameMapAccessor);
        var entityToMove = new TestEntity(gameMapAccessor);

        map.AddEntityToMap(primaryEntity, 1);
        map.AddEntityToMap(entityToMove, 1);

        // Act
        var result = map.MoveEntityTo(primaryEntity, entityToMove, 10);

        // Assert
        Assert.AreEqual(MoveResult.IllegalMove, result, "Expected MoveResult.IllegalMove when the move is not legal.");
    }

    [TestMethod]
    public void MoveEntityTo_CannotMoveToSameRoom_ReturnsCannotMoveToSameRoom()
    {
        // Arrange
        var map = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(map);
        var primaryEntity = new TestEntity(gameMapAccessor);
        var entityToMove = new TestEntity(gameMapAccessor);

        map.AddEntityToMap(primaryEntity, 1);
        map.AddEntityToMap(entityToMove, 1);

        // Act
        var result = map.MoveEntityTo(primaryEntity, entityToMove, 1);

        // Assert
        Assert.AreEqual(MoveResult.CannotMoveToSameRoom, result, "Expected MoveResult.CannotMoveToSameRoom when trying to move an entity to the same room.");
    }

    [TestMethod]
    public void MoveEntityTo_SuccessfulMove_ReturnsSuccess()
    {
        // Arrange
        var map = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(map);
        var primaryEntity = new TestEntity(gameMapAccessor);
        var entityToMove = new TestEntity(gameMapAccessor);

        map.AddEntityToMap(primaryEntity, 1);
        map.AddEntityToMap(entityToMove, 1);

        // Act
        var result = map.MoveEntityTo(primaryEntity, entityToMove, 2);

        // Assert
        Assert.AreEqual(MoveResult.Success, result, "Expected MoveResult.Success when the move is successful.");
    }

    [TestMethod]
    public void MoveEntityTo_CanMoveEntitiesToAnyRoom_ReturnsSuccess()
    {
        // Arrange
        var map = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(map);
        var primaryEntity = new TestEntity(gameMapAccessor)
        {
            CanMoveOtherEntitiesOverride = true
        };
        var entityToMove = new TestEntity(gameMapAccessor);
        map.AddEntityToMap(primaryEntity, 1);
        map.AddEntityToMap(entityToMove, 1);

        // Act
        var result = map.MoveEntityTo(primaryEntity, entityToMove, 4);

        // Assert
        Assert.AreEqual(MoveResult.Success, result, "Expected to return success when primary entity can move other entities to any room.");
    }

}
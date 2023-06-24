using WumpusLib.Entities;
using WumpusLib.Map.Results;

namespace WumpusLib.Tests.GameMap;

[TestClass]
public class MoveToUnitTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MoveTo_NullEntity_ThrowsArgumentNullException()
    {
        var map = new TestGameMap();
        map.MoveTo(null, 2);
    }

    [TestMethod]
    public void MoveTo_EntityCannotMove_ReturnsEntityNotAllowedToMove()
    {
        // Arrange
        var map = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(map);
        var entity = new TestEntity(gameMapAccessor)
        {
            CanMoveOverride = false
        };

        map.AddEntityToMap(entity, 1);

        // Act
        var result = map.MoveTo(entity, 2);

        // Assert
        Assert.AreEqual(MoveResult.EntityNotAllowedToMove, result, "Expected MoveResult.EntityNotAllowedToMove when entity cannot move.");
    }

    [TestMethod]
    public void MoveTo_EntityDead_ReturnsEntityNotAllowedToMove()
    {
        // Arrange
        var map = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(map);
        var entity = new TestEntity(gameMapAccessor)
        {
            CustomHealth = 0
        };
        map.AddEntityToMap(entity, 1);

        // Act
        var result = map.MoveTo(entity, 2);

        // Assert
        Assert.AreEqual(MoveResult.EntityNotAllowedToMove, result, "Expected MoveResult.EntityNotAllowedToMove when entity is dead.");
    }

    [TestMethod]
    public void MoveTo_EntityCannotMoveToAnyRoomAndRoomsNotLinked_ReturnsIllegalMove()
    {
        // Arrange
        var map = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(map);
        var entity = new TestEntity(gameMapAccessor)
        {
            CanMoveToAnyRoomOverride = false
        };

        map.AddEntityToMap(entity, 1);

        // Act
        var result = map.MoveTo(entity, 3); // Assuming rooms 1 and 3 are not directly linked

        // Assert
        Assert.AreEqual(MoveResult.IllegalMove, result, "Expected MoveResult.IllegalMove when entity cannot move to any room and rooms are not linked.");
    }

    [TestMethod]
    public void MoveTo_RoomDoesNotExist_ReturnsInvalidRoomNumber()
    {
        // Arrange
        var map = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(map);
        var entity = new TestEntity(gameMapAccessor);
        map.AddEntityToMap(entity, 1);

        // Act
        var result = map.MoveTo(entity, 999); // Room 999 does not exist

        // Assert
        Assert.AreEqual(MoveResult.InvalidRoomNumber, result, "Expected MoveResult.InvalidRoomNumber when room does not exist.");
    }

    [TestMethod]
    public void MoveTo_MoveToSameRoom_ReturnsCannotMoveToSameRoom()
    {
        // Arrange
        var map = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(map);
        var entity = new TestEntity(gameMapAccessor);
        map.AddEntityToMap(entity, 1);

        // Act
        var result = map.MoveTo(entity, 1);

        // Assert
        Assert.AreEqual(MoveResult.CannotMoveToSameRoom, result, "Expected MoveResult.CannotMoveToSameRoom when trying to move to the same room.");
    }

    [TestMethod]
    public void MoveTo_MoveToDifferentRoom_Success()
    {
        // Arrange
        var map = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(map);
        var entity = new TestEntity(gameMapAccessor);
        map.AddEntityToMap(entity, 1);

        // Act
        var result = map.MoveTo(entity, 2); // Assuming rooms 1 and 2 are directly linked

        // Assert
        Assert.AreEqual(MoveResult.Success, result, "Expected MoveResult.Success when moving to a different, linked room.");
    }

}
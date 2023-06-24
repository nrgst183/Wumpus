using WumpusLib.Map.Results;

namespace WumpusLib.Tests.GameMap;

[TestClass]
public class LinkRoomsUnitTests
{
    [TestMethod]
    public void LinkRooms_ValidRooms_LinkRoomsSuccessfully()
    {
        // Arrange
        var gameMap = new TestGameMap();

        var room1 = 1;
        var room2 = 2;

        // Act
        var linkResult = gameMap.LinkRoomsPublic(room1, room2);

        // Assert
        Assert.AreEqual(LinkResult.Success, linkResult, "Linking two valid rooms should return Success");
        Assert.IsTrue(gameMap.RoomLinks[room1].Contains(room2), "Room 1 should be linked with room 2");
        Assert.IsTrue(gameMap.RoomLinks[room2].Contains(room1), "Room 2 should be linked with room 1");
    }

    [TestMethod]
    public void LinkRooms_AlreadyLinkedRooms_ReturnsRoomsAlreadyLinked()
    {
        // Arrange
        var gameMap = new TestGameMap();

        var room1 = 1;
        var room2 = 2;

        gameMap.LinkRoomsPublic(room1, room2);

        // Act
        var linkResult = gameMap.LinkRoomsPublic(room1, room2);

        // Assert
        Assert.AreEqual(LinkResult.RoomsAlreadyLinked, linkResult,
            "Linking already linked rooms should return RoomsAlreadyLinked");
    }
}
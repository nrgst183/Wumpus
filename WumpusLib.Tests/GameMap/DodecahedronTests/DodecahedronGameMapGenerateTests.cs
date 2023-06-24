using WumpusLib.Entities;
using WumpusLib.Entities.Player;
using WumpusLib.Game;
using WumpusLib.Game.GameModes;
using WumpusLib.Map;

namespace WumpusLib.Tests.GameMap.DodecahedronTests;

[TestClass]
public class DodecahedronGameMapGenerateTests
{
    [TestMethod]
    public void Generate_MapStructure_CorrectNumberOfRooms()
    {
        // Arrange
        var gameMap = new DodecahedronGameMap(20);

        // Act
        gameMap.InitializeGameMap(new TestGameMode(new GameModeOptions()));

        // Assert
        Assert.AreEqual(gameMap.AmountOfRooms, gameMap.RoomLinks.Count, "Number of rooms should match AmountOfRooms");
    }

    [TestMethod]
    public void Generate_MapStructure_ValidRoomLinks()
    {
        // Arrange
        var gameMap = new DodecahedronGameMap(20);
        // Act
        gameMap.InitializeGameMap(new TestGameMode(new GameModeOptions()));

        // Assert
        foreach (var room in gameMap.RoomLinks.Keys)
        {
            Assert.IsTrue(gameMap.RoomLinks[room].Count >= gameMap.AmountOfTunnels, $"Room {room} should have at least {gameMap.AmountOfTunnels} links");

            foreach (var linkedRoom in gameMap.RoomLinks[room])
            {
                Assert.IsTrue(gameMap.RoomLinks[linkedRoom].Contains(room), $"Room {linkedRoom} should be linked with room {room}");
            }
        }
    }

    [TestMethod]
    public void Generate_MapStructure_IsMapGeneratedTrue()
    {
        var gameMap = new DodecahedronGameMap(20);

        // Act
        gameMap.InitializeGameMap(new TestGameMode(new GameModeOptions()));

        // Assert
        Assert.IsTrue(gameMap.IsMapGenerated, "IsMapGenerated should be true after Generate method call");
    }
}
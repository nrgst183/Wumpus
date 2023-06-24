using WumpusLib.Entities;
using WumpusLib.Map;
using WumpusLib.Map.Results;

namespace WumpusLib.Tests.GameMap;

[TestClass]
public class AddEntityToRandomRoomUnitTests
{
    [TestMethod]
    public void AddEntityToRandomRoom_Succeeds_WhenCompatibleRoomsExist()
    {
        // Arrange
        var map = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(map);
        map.BuildGameMapPublic();
        var entityToAdd = new TestEntity(gameMapAccessor);

        // Act
        var result = map.AddEntityToRandomRoom(entityToAdd, 1);

        // Assert
        Assert.AreEqual(MoveResult.Success, result);
    }

    [TestMethod]
    public void AddEntityToRandomRoom_EntitiesDoNotOverlap_WithIncompatibleSpawning()
    {
        // Arrange
        const int iterations = 50;
        bool entitiesOverlap = false;

        for (int i = 0; i < iterations; i++)
        {
            var testGameMap = new TestGameMap(20);
            testGameMap.TestEntitySpawnParameters.Add(new EntitySpawnParameters(typeof(TestEntity), 1, 1, new List<Type> { typeof(TestEntity) } ));

            var playerAccessor = new EntityGameMapAccessor(testGameMap);
            var player = new TestEntity(playerAccessor);
            var incompatibleEntityAccessor = new EntityGameMapAccessor(testGameMap);
            var incompatibleEntity = new TestEntity(incompatibleEntityAccessor);

            // Act
            testGameMap.AddEntityToRandomRoom(player, 1);
            testGameMap.AddEntityToRandomRoom(incompatibleEntity, 1);

            // Assert
            var playerRoomNumber = testGameMap.GetRoomNumberForEntity(player);
            var incompatibleEntityRoomNumber = testGameMap.GetRoomNumberForEntity(incompatibleEntity);
            var visitedRooms = Extensions.BfsTraverseNodesByDegreesOfSeparation(playerRoomNumber, testGameMap.RoomLinks, 0).Keys;

            if (!visitedRooms.Contains(incompatibleEntityRoomNumber))
                continue;

            entitiesOverlap = true;
            break;
        }

        Assert.IsFalse(entitiesOverlap, "The entities overlapped in one of the iterations.");
    }

    [DataTestMethod]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    public void AddEntityToRandomRoom_WithIncompatibleEntity_MultipleDegreesOfSeparation(int degreeOfSeparation)
    {
        // Arrange
        int roomCount = degreeOfSeparation * 10;
        var testGameMap = new TestGameMap(roomCount);
        testGameMap.TestEntitySpawnParameters.Add(new EntitySpawnParameters(typeof(TestEntity), 1, 1, new List<Type> { typeof(TestEntity) }));

        var playerAccessor = new EntityGameMapAccessor(testGameMap);
        var player = new TestEntity(playerAccessor);
        var incompatibleEntityAccessor = new EntityGameMapAccessor(testGameMap);
        var incompatibleEntity = new TestEntity(incompatibleEntityAccessor);

        // Act
        testGameMap.AddEntityToRandomRoom(player, degreeOfSeparation);
        testGameMap.AddEntityToRandomRoom(incompatibleEntity, degreeOfSeparation);

        // Assert
        var playerRoomNumber = testGameMap.GetRoomNumberForEntity(player);
        var incompatibleRoomNumber = testGameMap.GetRoomNumberForEntity(incompatibleEntity);
        var visitedRooms = Extensions.BfsTraverseNodesByDegreesOfSeparation(playerRoomNumber, testGameMap.RoomLinks, degreeOfSeparation - 1).Keys;

        Assert.AreNotEqual(playerRoomNumber, incompatibleRoomNumber, "Player and incompatible entity should not be in the same room.");
        Assert.IsFalse(visitedRooms.Contains(incompatibleRoomNumber), $"Incompatible entity should be at least {degreeOfSeparation} rooms away from the player.");
    }

}
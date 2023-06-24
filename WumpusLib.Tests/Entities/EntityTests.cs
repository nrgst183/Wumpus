using WumpusLib.Entities;

namespace WumpusLib.Tests.Entities;

[TestClass]
public class EntityTests
{
    [TestMethod]
    public void Entity_InflictDamage()
    {
        // Arrange
        var gameMap = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(gameMap);
        var attackerEntity = new TestEntity(gameMapAccessor) {CustomHealth = 20};
        var targetEntity = new TestEntity(gameMapAccessor) {CustomHealth = 30};

        // Act
        var damageDealt = attackerEntity.InflictDamage(targetEntity);
        var targetHealth = targetEntity.Health;

        // Assert
        Assert.AreEqual<int>(0,
            damageDealt); // TestEntity has CanAttackOtherEntities set to false, so no damage should be dealt
        Assert.AreEqual(30, targetHealth); // Target entity's health should remain unchanged
    }

    [TestMethod]
    public void Entity_ReceiveDamage()
    {
        // Arrange
        var gameMap = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(gameMap);
        var attackerEntity = new TestEntity(gameMapAccessor) {CustomHealth = 20};
        var targetEntity = new TestEntity(gameMapAccessor) {CustomHealth = 30};

        // Act
        var damageReceived = targetEntity.ReceiveDamage(attackerEntity, 10);
        var targetHealth = targetEntity.Health;

        // Assert
        Assert.AreEqual<int>(10, damageReceived); // Target entity should receive 10 points of damage
        Assert.AreEqual(20, targetHealth); // Target entity's health should be reduced by 10 points
    }

    [TestMethod]
    public void TestEntity_ReceiveDamage_InvalidInput()
    {
        // Arrange
        var gameMap = new TestGameMap();
        var gameMapAccessor = new EntityGameMapAccessor(gameMap);
        var targetEntity = new TestEntity(gameMapAccessor) {CustomHealth = 30};

        // Act
        var damageReceived = targetEntity.ReceiveDamage(null, -5);
        var targetHealth = targetEntity.Health;

        // Assert
        Assert.AreEqual(0, damageReceived); // No damage should be received as the input is invalid
        Assert.AreEqual(30, targetHealth); // Target entity's health should remain unchanged
    }
}
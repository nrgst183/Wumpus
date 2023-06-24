namespace WumpusLib.Tests.ExtensionTests;

[TestClass]
public class BfsTraverseNodesByDegreesOfSeparationTests
{
    private static IDictionary<int, List<int>> CreateTestGraph()
    {
        return new Dictionary<int, List<int>>
        {
            {1, new List<int> {2, 3}},
            {2, new List<int> {1, 4, 5}},
            {3, new List<int> {1}},
            {4, new List<int> {2}},
            {5, new List<int> {2, 6}},
            {6, new List<int> {5}}
        };
    }

    [TestMethod]
    public void BfsTraverseNodesByDegreesOfSeparation_ReturnsAllNodes_WhenNoDepthLimit()
    {
        // Arrange
        var graph = CreateTestGraph();

        // Act
        var result = Extensions.BfsTraverseNodesByDegreesOfSeparation(1, graph, 3);

        // Assert
        Assert.AreEqual(6, result.Count);
    }

    [TestMethod]
    public void BfsTraverseNodesByDegreesOfSeparation_ReturnsOnlyNodesAtSpecificDepth()
    {
        // Arrange
        var graph = CreateTestGraph();

        // Act
        var result = Extensions.BfsTraverseNodesByDegreesOfSeparation(1, graph, 2, true);

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.IsFalse(result.ContainsKey(6));
        Assert.IsTrue(result.ContainsKey(4));
        Assert.IsTrue(result.ContainsKey(5));
    }

    [TestMethod]
    public void BfsTraverseNodesByDegreesOfSeparation_ReturnsEmpty_WhenDepthGreaterThanMaxDepth()
    {
        // Arrange
        var graph = CreateTestGraph();

        // Act
        var result = Extensions.BfsTraverseNodesByDegreesOfSeparation(1, graph, 4, true);

        // Assert
        Assert.AreEqual(0, result.Count);
    }
}
namespace WumpusLib.Tests.ExtensionTests;

[TestClass]
public class DijkstraTests
{
    private static IDictionary<int, List<int>> CreateTestGraph()
    {
        return new Dictionary<int, List<int>>
        {
            {1, new List<int> {2, 3}},
            {2, new List<int> {1, 3, 4}},
            {3, new List<int> {1, 2, 4, 5}},
            {4, new List<int> {2, 3, 5}},
            {5, new List<int> {3, 4}}
        };
    }

    [TestMethod]
    public void DijkstraGetShortestPathTo_ReturnsShortestPath()
    {
        // Arrange
        var graph = CreateTestGraph();

        // Act
        var result = Extensions.DijkstraGetShortestPathTo(1, 5, graph);

        // Assert
        CollectionAssert.AreEqual(new List<int> { 1, 3, 5 }, result.ToList());
    }

    [TestMethod]
    public void DijkstraGetShortestPathTo_ReturnsShortestPathWithExcludedNodes()
    {
        // Arrange
        var graph = CreateTestGraph();
        var excludeList = new List<int> { 3 };

        // Act
        var result = Extensions.DijkstraGetShortestPathTo(1, 5, graph, excludeList);

        // Assert
        CollectionAssert.AreEqual(new List<int> { 1, 2, 4, 5 }, result.ToList());
    }

    [TestMethod]
    public void DijkstraGetShortestPathTo_ReturnsNoValidPathDueToExcludedNodes()
    {
        // Arrange
        var graph = CreateTestGraph();
        var excludeList = new List<int> { 2, 3, 4 };

        // Act
        var result = Extensions.DijkstraGetShortestPathTo(1, 5, graph, excludeList);

        // Assert

        CollectionAssert.AreEqual(new List<int>(), result);
    }
}
using GenericPathfinding.Source;
    
namespace GenericPathfinding.Tests
{
    [TestClass]
    public class StraightLineGraphTests
    {
        private class Graph : IGraph<int>
        {
            public bool IsNode(int node)
            {
                return 0 <= node && node <= 100;
            }

            public float GetEdgeWeight(int nodeA, int nodeB)
            {
                return 1.0f;
            }

            public float GetHeuristicDistance(int node, int goalNode)
            {
                return Math.Abs(goalNode - node);
            }

            public IEnumerable<int> GetNeighbors(int node)
            {
                if (node == 0)
                    return new List<int>() { 1 };
                else if (node == 100)
                    return new List<int>() { 99 };
                else
                    return new List<int>() { node - 1, node + 1 };
            }
        }

        [TestMethod]
        public void TestAStar()
        {
            IGraph<int> graph = new Graph();
            AStarPathfinder<int> pf = new(graph, 0, 100);

            bool foundPath = pf.RunToEnd(out PathInfo<int>? pathInfo);

            Assert.IsTrue(foundPath);
            Assert.IsNotNull(pathInfo);

            Assert.AreEqual(pathInfo.Length, 100);
            IList<int> seq = pathInfo.Path;
            Assert.AreEqual(seq.Count, 101);
            Assert.AreEqual(seq[0], 0);
            Assert.AreEqual(seq[50], 50);
            Assert.AreEqual(seq[100], 100);
        }


        [TestMethod]
        public void TestNodeLimit()
        {
            IGraph<int> graph = new Graph();
            AStarPathfinder<int> pf = new(graph, 0, 100) { NodeLimit = 95 };
            bool foundPath = pf.RunToEnd(out PathInfo<int>? _);
            Assert.IsFalse(foundPath);
        }

        [TestMethod]
        public void TestDistanceLimit()
        {
            IGraph<int> graph = new Graph();
            AStarPathfinder<int> pf = new(graph, 0, 100) { DistanceLimit = 95.0f };
            bool foundPath = pf.RunToEnd(out PathInfo<int>? _);
            Assert.IsFalse(foundPath);
        }

    }
}
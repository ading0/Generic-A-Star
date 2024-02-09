using GenericPathfinding.Source;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GenericPathfinding.Tests
{
    [TestClass]
    public class IslandGraphTests
    {
        private static readonly float Sqrt2 = MathF.Sqrt(2);

        private class ConnectedIslands : IGraph<(int, int)>
        {
            public float GetEdgeWeight((int, int) nodeA, (int, int) nodeB)
            {
                return 1;
            }

            public float GetHeuristicDistance((int, int) node, (int, int) goalNode)
            {
                float dx = node.Item1 - goalNode.Item1;
                float dy = node.Item2 - goalNode.Item2;
                return Math.Abs(dx) + Math.Abs(dy);
            }

            public IEnumerable<(int, int)> GetNeighbors((int, int) node)
            {
                int x = node.Item1;
                int y = node.Item2;
                return new List<(int, int)> { (x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1) }.Where(IsNode);
            }

            public bool IsNode((int, int) node)
            {
                int x = node.Item1;
                int y = node.Item2;
                if (y == 0)
                {
                    return x == 0;
                }
                else if (1 <= Math.Abs(y) && Math.Abs(y) <= 11)
                {
                    return Math.Abs(x) <= 5;
                }
                else
                {
                    return false;
                }
            }
        }

        private class TwoIslands : IGraph<(int, int)>
        {
            public float GetEdgeWeight((int, int) nodeA, (int, int) nodeB)
            {
                return 1;
            }

            public float GetHeuristicDistance((int, int) node, (int, int) goalNode)
            {
                float dx = node.Item1 - goalNode.Item1;
                float dy = node.Item2 - goalNode.Item2;
                return Math.Abs(dx) + Math.Abs(dy);
            }

            public IEnumerable<(int, int)> GetNeighbors((int, int) node)
            {
                int x = node.Item1;
                int y = node.Item2;
                return new List<(int, int)> { (x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1) }.Where(IsNode);
            }

            public bool IsNode((int, int) node)
            {
                int x = node.Item1;
                int y = node.Item2;
                if (1 <= Math.Abs(y) && Math.Abs(y) <= 11)
                {
                    return Math.Abs(x) <= 5;
                }
                else
                {
                    return false;
                }
            }
        }

        [TestMethod]
        public void TestAStarReachable()
        {
            IGraph<(int, int)> graph = new ConnectedIslands();
            AStarPathfinder<(int, int)> pf = new(graph, (-4, -3), (-4, 3));

            bool foundPath = pf.RunToEnd(out PathInfo<(int, int)>? pathInfo);

            Assert.IsTrue(foundPath);
            Assert.IsNotNull(pathInfo);

            IList<(int, int)> seq = pathInfo.Path;
            Assert.AreEqual(pathInfo.Length, 14);
            Assert.AreEqual(seq[0], (-4, -3));
            Assert.AreEqual(seq[7], (0, 0));
            Assert.AreEqual(seq[14], (-4, 3));
        }

        [TestMethod]
        public void TestAStarUnreachable()
        {
            IGraph<(int, int)> graph = new TwoIslands();
            AStarPathfinder<(int, int)> pf = new(graph, (0, -1), (0, 1));
            bool foundPath = pf.RunToEnd(out _);
            Assert.IsFalse(foundPath);
        }
    }
}

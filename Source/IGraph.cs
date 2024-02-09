using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericPathfinding.Source
{
    /* An interface for a (lazily-defined) graph. */
    public interface IGraph<N> where N : IEquatable<N>
    {
        public bool IsNode(N node);

        public IEnumerable<N> GetNeighbors(N node);

        // nodeA and nodeB must be neighbors
        public float GetEdgeWeight(N nodeA, N nodeB);

        public float GetHeuristicDistance(N node, N goalNode);
    }
}

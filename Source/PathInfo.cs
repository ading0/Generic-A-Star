using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericPathfinding.Source
{
    public class PathInfo<N> where N : IEquatable<N>
    {
        public PathInfo(IList<N> path, float distance)
        {
            if (path is null || path.Count == 0)
                throw new ArgumentException("Must have at least one node in the path.", nameof(path));

            Path = path;

            if (distance < 0)
                throw new ArgumentException("Distance is negative.", nameof(distance));

            Distance = distance;
        }

        // Should include both start and end cells (only once if they coincide).
        public IList<N> Path { get; } 
        public float Distance { get; }
        public int Length => Path.Count - 1;
        public N End => Path[Length];

    }
}

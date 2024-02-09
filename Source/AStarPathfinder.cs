using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericPathfinding.Source
{
    /* Represents a specific pathfinding task.
     * Can be run incrementally using 'Run()' or all at once with 'RunToEnd()'. 
     * Use the 'Status' property to determine the status of the task. 'RunToEnd()' always results in 'Found' or 'NotFound'. 
     * If 'Status' is 'Found', the path can be found in 'PathInfo'. */
    public class AStarPathfinder<N> where N : IEquatable<N>
    {
        /* Constructor with a single goal. */
        public AStarPathfinder(IGraph<N> graph, N start, N goal)
        {
            Graph = graph;
            Start = start;
            RefGoal = goal;
            Ends = [goal];
        }

        /* Constructor for multiple possible end states. One node must still be specified as the goal. 
         * The goal does not have to be an end state, or even a valid node in the graph,
         *   although the heuristic will become inadmissible if so. */
        public AStarPathfinder(IGraph<N> graph, N start, ICollection<N> ends, N refGoal)
        {
            Graph = graph;
            Start = start;
            Ends = [.. ends];
            RefGoal = refGoal;
        }

        public IGraph<N> Graph { get; }
        public N Start { get; }
        public HashSet<N> Ends { get; }
        public N RefGoal { get; }

        /* Multiplies the heuristic by this factor.
         * Should only use values that are at least 1.
         * A value of exactly 1 is the admissible heuristic; higher than 1 is inadmissible but faster. */
        public float Relaxation { get; set; } = 1.0f;

        /* No more than this number of nodes can be dequeued from the open set.
         * The path is 'NotFound' if the limit is reached. */
        public int NodeLimit { get; set; } = int.MaxValue;

        // Nodes beyond this distance from 'Start' will not be explored.
        public float DistanceLimit { get; set; } = float.MaxValue;
        public PathfindingStatus Status { get; private set; } = PathfindingStatus.NotStarted;
        public PathInfo<N>? FoundPathInfo { get; private set; } = null;

        private readonly Dictionary<N, float> _costFromSource = [];
        private readonly PriorityQueue<N, float> _openSet = new();
        private readonly Dictionary<N, N> _predecessors = [];
        private readonly HashSet<N> _dequeued = [];

        public bool RunToEnd(out PathInfo<N>? pathInfo)
        {
            if (Status == PathfindingStatus.NotStarted)
                Initialize();

            while (Status == PathfindingStatus.Running)
                Run();

            if (Status == PathfindingStatus.Found)
            {
                pathInfo = FoundPathInfo;
                return true;
            }
            else
            {
                pathInfo = null;
                return false;
            }
        }

        public PathfindingStatus Run()
        {
            if (Status == PathfindingStatus.NotStarted)
                Initialize();  // run once

            if (Status != PathfindingStatus.Running)
                return Status; // already done

            if (_openSet.Count > 0)
            {
                if (_dequeued.Count >= NodeLimit)
                    return Status = PathfindingStatus.NotFound;

                N node = _openSet.Dequeue();
                if (_dequeued.Contains(node))
                    return Status = PathfindingStatus.Running;

                _dequeued.Add(node);

                if (Ends.Contains(node))
                {
                    // Reconstruct path and terminate with Found
                    List<N> path = [node];
                    N it = node;
                    while (_predecessors.ContainsKey(it))
                    {
                        it = _predecessors[it];
                        path.Add(it);
                    }
                    path.Reverse();

                    FoundPathInfo = new(path, _costFromSource[node]);
                    return Status = PathfindingStatus.Found;
                }

                // Update neighbors and return Running.
                foreach (N neighbor in Graph.GetNeighbors(node))
                {
                    float newCostFromSrc = _costFromSource[node] + Graph.GetEdgeWeight(node, neighbor);
                    if (newCostFromSrc > DistanceLimit)
                        continue;

                    if (!_costFromSource.TryGetValue(neighbor, out float oldCostFromSrc) || newCostFromSrc < oldCostFromSrc)
                    {
                        _costFromSource[neighbor] = newCostFromSrc;
                        _openSet.Enqueue(neighbor, newCostFromSrc + Relaxation * Graph.GetHeuristicDistance(neighbor, RefGoal));
                        _predecessors[neighbor] = node;
                    }
                }

                return Status = PathfindingStatus.Running;
            }

            // Open set was emptied without finding path.
            return Status = PathfindingStatus.NotFound;
        }

        private void Initialize()
        {
            if (Ends.Contains(Start))
            {
                FoundPathInfo = new([Start], 0);
                return;
            }

            _costFromSource.Add(Start, 0);
            _openSet.Enqueue(Start, Relaxation * Graph.GetHeuristicDistance(Start, RefGoal));

            Status = PathfindingStatus.Running;
        }
    }
}

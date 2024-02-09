## Generic A* Implementation

Written in Visual Studio 2022 (.NET 8).

#### Features

This is some pragmatic base code for A* pathfinding on discrete graphs, using only managed C#. I tried to add everything useful that can be cheaply added and doesn't require specific knowledge of the problem.

Some features over the basic A* algorithm that might be of interest:
- the ability to run A* incrementally
- support for lazily defined graphs (via an interface `IGraph`), include infinite graphs
- node and distance limits to terminate early to avoid searching too far
- multiple goal nodes

#### Usage

Copy the .cs files in the `source` subdirectory.

#### Limitations

The most conspicuous missing features are hierarchical path-finding and navmesh support; both require maintaining separate data structures and are hard to do without specific knowledge of the application.

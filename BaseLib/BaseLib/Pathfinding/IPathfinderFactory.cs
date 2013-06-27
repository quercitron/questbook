using BaseLib.Enumerables;

namespace BaseLib.Pathfinding
{
    interface IPathfinderFactory
    {
        IPathFinder Create(Book book, SearchAlgorithm algorithm);
    }
}

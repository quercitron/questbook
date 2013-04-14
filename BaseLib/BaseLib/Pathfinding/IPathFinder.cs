using System.Collections.Generic;

namespace BaseLib.Pathfinding
{
    public interface IPathFinder
    {
        List<SearchResultState> FindPath(PersonState startState, Dictionary<int, List<Edge>> edges);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseLib.Pathfinding;

namespace BaseLib
{
    public class FastSearchPathfinder : IPathFinder
    {
        public FastSearchPathfinder(Book book)
        {

        }

        public List<SearchResultState> FindPath(PersonState startState, Dictionary<int, List<Edge>> edges)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using BaseLib.Pathfinding.PathFormators;
using BaseLib.Pathfinding.PathStateGenerators;

namespace BaseLib.Pathfinding
{
    public class BasePathFinder : IPathFinder
    {
        public BasePathFinder(IPathStateGenerator pathStateGenerator, IPathFormator pathFormator)
        {
            m_PathStateGenerator = pathStateGenerator;
            m_PathFormator = pathFormator;
        }

        private readonly IPathStateGenerator m_PathStateGenerator;

        private readonly IPathFormator m_PathFormator;

        public List<SearchResultState> FindPath(PersonState startState, Dictionary<int, List<Edge>> edges)
        {
            try
            {
                var states = m_PathStateGenerator.GenerateStates(startState, edges);
                var path = m_PathFormator.FormPath(states);
                return path;
            }
            finally
            {
                // TODO: Think about GC
                GC.Collect();
            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using BaseLib.Comparers;

namespace BaseLib.Pathfinding.PathStateGenerators
{
    public abstract class BasePathStateGenerator : IPathStateGenerator
    {
        abstract public Dictionary<PersonState, int> GenerateStates(PersonState startState, Dictionary<int, List<Edge>> edges);

        virtual protected List<PersonState> GenerateNextStates(PersonState state, Dictionary<int, List<Edge>> edges)
        {
            var result = new List<PersonState>();

            foreach (var edge in edges[state.ParagraphNo].Where(e => e.IsAcite).OrderByDescending(e => e.Priority, new PriorityComparer()))
            {
                if (ItemsWorker.EdgeIsAvailable(state, edge))
                {
                    var newState = ItemsWorker.MoveAlongTheEdge(state, edge);
                    if (newState != null)
                    {
                        result.Add(newState);
                    }
                    if (edge.Priority > 0)
                    {
                        break;
                    }
                }
            }

            return result;
        }
    }
}

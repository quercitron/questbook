using System.Collections.Generic;

namespace BaseLib.Pathfinding.PathStateGenerators
{
    public interface IPathStateGenerator
    {
        Dictionary<PersonState, int> GenerateStates(PersonState startState, Dictionary<int, List<Edge>> edges);
    }
}

using System.Collections.Generic;
using System.Linq;
using BaseLib.Extensions;

namespace BaseLib.Pathfinding.PathStateGenerators
{
    public class DfsPathStateGenerator : BasePathStateGenerator
    {
        public DfsPathStateGenerator(bool shuffle)
        {
            Shuffle = shuffle;
        }

        public bool Shuffle { get; private set; }

        public override Dictionary<PersonState, int> GenerateStates(PersonState startState, Dictionary<int, List<Edge>> edges)
        {
            var stateDict = new Dictionary<PersonState, int>();

            Dfs(startState, edges, 0, stateDict, Shuffle);

            return stateDict;
        }

        private void Dfs(PersonState current, Dictionary<int, List<Edge>> edges, int distance, Dictionary<PersonState, int> stateDict, bool shuffle)
        {
            stateDict.Add(current, distance);

            var nextStates = GenerateNextStates(current, edges).ToList();

            if (shuffle)
            {
                nextStates.Shuffle();
            }

            foreach (var nextState in nextStates)
            {
                if (!stateDict.ContainsKey(nextState))
                {
                    Dfs(nextState, edges, distance + 1, stateDict, shuffle);
                }
            }
        }
    }
}

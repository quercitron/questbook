using System.Collections.Generic;
using System.Linq;

namespace BaseLib.Pathfinding.PathStateGenerators
{
    public class BfsPathStateGenerator : BasePathStateGenerator
    {
        public override Dictionary<PersonState, int> GenerateStates(PersonState startState, Dictionary<int, List<Edge>> edges)
        {
            var stateDict = new Dictionary<PersonState, int>();
            stateDict.Add(startState, 0);

            var queue = new Queue<PersonState>();
            queue.Enqueue(startState);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var currentDist = stateDict[current];

                var nextStates = GenerateNextStates(current, edges).Where(s => !stateDict.ContainsKey(s));
                foreach (var nextState in nextStates)
                {
                    stateDict.Add(nextState, currentDist + 1);
                    queue.Enqueue(nextState);
                }
            }

            return stateDict;
        }
    }
}

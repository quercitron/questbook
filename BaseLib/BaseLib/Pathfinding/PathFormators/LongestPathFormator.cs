using System.Collections.Generic;
using System.Linq;

namespace BaseLib.Pathfinding.PathFormators
{
    public class LongestPathFormator : BasePathFormator
    {
        public override List<SearchResultState> FormPath(Dictionary<PersonState, int> states)
        {
            var max = states.Values.Max();
            var furthestState = states.First(p => p.Value == max).Key;

            return PathToState(states, furthestState);
        }
    }
}

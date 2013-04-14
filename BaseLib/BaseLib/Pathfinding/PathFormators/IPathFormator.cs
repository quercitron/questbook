using System.Collections.Generic;

namespace BaseLib.Pathfinding.PathFormators
{
    public interface IPathFormator
    {
        List<SearchResultState> FormPath(Dictionary<PersonState, int> states);
    }
}

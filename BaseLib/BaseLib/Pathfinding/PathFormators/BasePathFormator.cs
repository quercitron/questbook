using System.Collections.Generic;

namespace BaseLib.Pathfinding.PathFormators
{
    public abstract class BasePathFormator : IPathFormator
    {
        public abstract List<SearchResultState> FormPath(Dictionary<PersonState, int> states);

        protected virtual List<SearchResultState> PathToState(IDictionary<PersonState, int> searchResult, PersonState finalState)
        {
            var result = new List<SearchResultState>();
            while (finalState != null)
            {
                result.Add(new SearchResultState(finalState, searchResult[finalState]));
                finalState = finalState.PreviousState;
            }

            result.Reverse();
            return result;
        }
    }
}
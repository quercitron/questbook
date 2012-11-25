using System;
using BaseLib.Enumerables;

namespace BaseLib
{
    [Serializable]
    public class SearchParameters
    {
        public SearchParameters(PersonState startState, SearchAlgorithm algorithm)
        {
            StartState = startState;
            Algorithm = algorithm;
        }

        public PersonState StartState { get; private set; }

        public SearchAlgorithm Algorithm { get; private set; }
    }
}

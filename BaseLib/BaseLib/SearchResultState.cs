using System;

namespace BaseLib
{
    [Serializable]
    public class SearchResultState
    {
        public SearchResultState(PersonState state, int distance)
        {
            State = state;
            Distance = distance;
        }

        public PersonState State { get; private set; }
        public int Distance { get; private set; }
        public bool IsSame { get; set; }
    }
}

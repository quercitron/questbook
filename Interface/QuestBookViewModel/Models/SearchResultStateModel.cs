using System.Collections.Generic;
using BaseLib;

namespace QuestBookViewModel.Models
{
    public class SearchResultStateModel : ObservableObject
    {
        public SearchResultStateModel(SearchResultState searchResultState)
        {
            m_SearchResultState = searchResultState;
            RaisePropertyChanged("Items");
        }

        private readonly SearchResultState m_SearchResultState;

        public PersonState State
        {
            get { return m_SearchResultState.State; }
        }

        public int Distance
        {
            get { return m_SearchResultState.Distance; }
        }

        public List<ItemUnit> Items
        {
            get { return m_SearchResultState.State.Items; }
        }

        public bool IsSame
        {
            get { return m_SearchResultState.IsSame; }
        }
    }
}

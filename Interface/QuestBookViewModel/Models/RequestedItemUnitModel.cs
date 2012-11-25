using BaseLib;

namespace QuestBookViewModel.Models
{
    public class RequestedItemUnitModel : ItemUnitModel
    {
        public RequestedItemUnitModel(ItemUnit itemUnit)
            : base(itemUnit)
        {
        }

        public bool IsDisappearing
        {
            get { return ((RequestedItemUnit) ItemUnit).IsDisappearing; }
            set
            {
                ((RequestedItemUnit) ItemUnit).IsDisappearing = value;
                RaisePropertyChanged("IsDisappearing");
            }
        }
    }
}

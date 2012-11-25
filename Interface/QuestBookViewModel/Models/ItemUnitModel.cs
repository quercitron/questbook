using BaseLib;

namespace QuestBookViewModel.Models
{
    public class ItemUnitModel : ObservableObject
    {
        public ItemUnitModel(ItemUnit itemUnit)
        {
            ItemUnit = itemUnit;
            m_ItemTypeModel = new ItemTypeModel(itemUnit.BasicItem);
        }

        public ItemUnit ItemUnit { get; private set; }
        private  ItemTypeModel m_ItemTypeModel;

        public ItemTypeModel BasicItem
        {
            get { return m_ItemTypeModel; }
            set
            {
                ItemUnit.BasicItem = value.ItemType;
                m_ItemTypeModel = value;
                RaisePropertyChanged("BasicItem");
            }
        }

        public int Count
        {
            get { return ItemUnit.Count; }
            set
            {
                ItemUnit.Count = value;
                RaisePropertyChanged("Count");
            }
        }

        public bool Equals(ItemUnitModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.ItemUnit, ItemUnit);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ItemUnitModel)) return false;
            return Equals((ItemUnitModel) obj);
        }

        public override int GetHashCode()
        {
            return (ItemUnit != null ? ItemUnit.GetHashCode() : 0);
        }
    }
}

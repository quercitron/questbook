using BaseLib;

namespace QuestBookViewModel.Models
{
    public class ItemTypeModel : ObservableObject
    {
        public ItemTypeModel(ItemType itemType)
        {
            ItemType = itemType;
        }

        public ItemType ItemType { get; set; }

        public string Name
        {
            get { return ItemType.Name; }
            set
            {
                ItemType.Name = value;
                RaisePropertyChanged("Name");
            }
        }

        public bool InUse
        {
            get { return ItemType.InUse; }
            set
            {
                ItemType.InUse = value;
                RaisePropertyChanged("InUse");
            }
        }

        public bool IsUnique
        {
            get { return ItemType.IsUnique; }
            set
            {
                ItemType.IsUnique = value;
                RaisePropertyChanged("IsUnique");
            }
        }

        public bool IsProhibiting
        {
            get { return ItemType.IsProhibiting; }
            set
            {
                ItemType.IsProhibiting = value;
                RaisePropertyChanged("IsProhibiting");
            }
        }

        public bool IsVital
        {
            get { return ItemType.IsVital; }
            set
            {
                ItemType.IsVital = value;
                RaisePropertyChanged("IsVital");
            }
        }

        public override string ToString()
        {
            return ItemType.ToString();
        }

        public bool Equals(ItemTypeModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.ItemType, ItemType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ItemTypeModel)) return false;
            return Equals((ItemTypeModel) obj);
        }

        public override int GetHashCode()
        {
            return (ItemType != null ? ItemType.GetHashCode() : 0);
        }
    }
}

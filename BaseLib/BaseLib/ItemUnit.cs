using System;

namespace BaseLib
{
    [Serializable]
    public class ItemUnit
    {
        public ItemUnit(ItemType item, int count)
        {
            this.BasicItem = item;
            this.Count = count;
        }

        public ItemUnit(ItemType basicItem)
            : this(basicItem, 1)
        {
        }
         
        public ItemUnit(ItemUnit item)
            : this(item.BasicItem, item.Count)
        {
        }

        public ItemType BasicItem { get; set; }

        public int Count { get; set; }

        public bool Equals(ItemUnit other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Equals(other.BasicItem, this.BasicItem) && other.Count == this.Count;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != typeof(ItemUnit))
            {
                return false;
            }
            return Equals((ItemUnit)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.BasicItem != null ? this.BasicItem.GetHashCode() : 0) * 397) ^ this.Count;
            }
        }

        public override string ToString()
        {
            return string.Format("({0}, Count: {1})", this.BasicItem, this.Count);
        }
    }
}

using System;

namespace BaseLib
{
    [Serializable]
    public class RequestedItemUnit : ItemUnit
    {
        public RequestedItemUnit(ItemType item, int count, bool isDisappearing)
            : base(item, count)
        {
            this.IsDisappearing = isDisappearing;
        }

        public RequestedItemUnit(ItemType item, int count)
            : this(item, count, true)
        {
        }

        public RequestedItemUnit(ItemType item, bool isDisappearing)
            : this(item, 1, isDisappearing)
        {
        }

        public RequestedItemUnit(ItemType item)
            : this(item, 1, true)
        {
        }

        public bool IsDisappearing { get; set; }
    }
}

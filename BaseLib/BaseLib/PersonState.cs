using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseLib
{
    [Serializable]
    public class PersonState
    {
        public PersonState(int paragraphNo, List<ItemUnit> items, PersonState previousState, Edge previousEdge)
        {
            this.Items = items;
            this.ParagraphNo = paragraphNo;
            this.PreviousState = previousState;
            this.PreviousEdge = previousEdge;

            unchecked
            {
                m_HashCode =
                    (this.Items.Where(item => item.BasicItem.InUse).Aggregate(0, (current, itemUnit) => current ^ (itemUnit.Count * 100003 + itemUnit.GetHashCode()))) ^
                    (1000003 * this.ParagraphNo);
            }
        }

        public PersonState(int paragraphNo, List<ItemUnit> items)
            : this(paragraphNo, items, null, null)
        {
        }

        public PersonState(int paragraphNo)
            : this(paragraphNo, new List<ItemUnit>(), null, null)
        {
        }

        public int ParagraphNo { get; private set; }

        public readonly List<ItemUnit> Items;

        public Edge PreviousEdge { get; set; }

        public PersonState PreviousState { get; set; }

        private readonly int m_HashCode;

        public bool Equals(PersonState other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other.ParagraphNo == this.ParagraphNo && ItemsWorker.ItemsAreEqual(other.Items, this.Items);
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
            if (obj.GetType() != typeof(PersonState))
            {
                return false;
            }
            return Equals((PersonState)obj);
        }

        public override int GetHashCode()
        {
            /*unchecked
            {
                int result = this.Items.Where(item => item.BasicItem.InUse).Aggregate(0, (current, itemUnit) => current ^ (itemUnit.Count * 100003 + itemUnit.GetHashCode()));
                return result ^ (1000003 * this.ParagraphNo);
            }*/

            return m_HashCode;
        }

        public override string ToString()
        {
            StringBuilder itemsList = new StringBuilder();
            foreach (var item in Items.Where(item => item.BasicItem.InUse))
            {
                itemsList.Append(item);
                if (item != Items.Last())
                {
                    itemsList.Append(", ");
                }
            }
            return string.Format("ParagraphNo: {0}, Items: {1}", this.ParagraphNo, itemsList);
        }
    }
}

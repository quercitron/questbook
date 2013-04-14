using System;

namespace BaseLib
{
    [Serializable]
    public class ItemType
    {
        public ItemType(string name, bool isUnique, bool inUse, bool isProhibiting, bool isVital)
        {
            Name = name;
            IsUnique = isUnique;
            InUse = inUse;
            IsProhibiting = isProhibiting;
            IsVital = isVital;
        }

        public ItemType(string name, bool isUnique, bool isVital)
            : this(name, isUnique, true, true, isVital)
        {
        }

        public string Name { get; set; }

        public bool IsUnique { get; set; }

        public bool InUse { get; set; }

        public bool IsProhibiting { get; set; }

        public bool IsVital { get; set; }

        public override string ToString()
        {
            return string.Format(this.Name);
        }
    }
}

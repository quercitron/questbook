using System;

namespace BaseLib
{
    [Serializable]
    public class ItemType
    {
        public ItemType(string name, bool isUnique, bool isVital, bool inUse)
        {
            this.Name = name;
            this.IsUnique = isUnique;
            this.IsVital = isVital;
            this.InUse = inUse;
        }

        public ItemType(string name, bool isUnique, bool isVital)
            : this(name, isUnique, isVital, true)
        {
        }

        public string Name { get; set; }

        public bool IsUnique { get; set; }

        public bool InUse { get; set; }

        public bool IsVital { get; set; }

        public override string ToString()
        {
            return string.Format(this.Name);
        }
    }
}

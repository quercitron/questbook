using System.Collections.Generic;

namespace FastSearch
{
    internal class PriorityComparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            if (x == 0 && y == 0)
            {
                return 0;
            }
            if (x == 0)
            {
                return -1;
            }
            if (y == 0)
            {
                return 1;
            }
            return -x.CompareTo(y);
        }
    }
}

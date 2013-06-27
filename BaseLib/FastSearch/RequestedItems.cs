using System;
using System.Collections.Generic;

namespace FastSearch
{
    public class RequestedItems
    {
        public Int64 Mask { get; set; }

        public Int64 UncMask { get; set; }

        public List<RequestedItem> CountableItems { get; set; }
    }
}

using System.Collections.Generic;

namespace BaseLib.Interfaces
{
    public interface IRequestItems
    {
        List<RequestedItemUnit> RequestedItems { get; }
        void AddRequestedItem(RequestedItemUnit requestedItem);
    }
}
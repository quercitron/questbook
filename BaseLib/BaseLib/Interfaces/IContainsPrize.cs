using System.Collections.Generic;

namespace BaseLib.Interfaces
{
    public interface IContainsPrize
    {
        List<ItemUnit> RecievedItems { get; }

        void AddRecievedItem(ItemUnit recievedItem);
    }
}
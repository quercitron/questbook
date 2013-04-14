using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BaseLib.Interfaces;

namespace BaseLib
{
    [Serializable]
    public class Paragraph : IContainsPrize, IRequestItems
    {
        public Paragraph(int id)
        {
            this.Id = id;

            RecievedItems = new List<ItemUnit>();
            RequestedItems = new List<RequestedItemUnit>();
        }

        public int Id { get; set; }

        public string Description { get; set; }

        [OptionalField] 
        private bool m_WasVisited;

        public bool WasVisited
        {
            get { return m_WasVisited; }
            set { m_WasVisited = value; }
        }

        public List<ItemUnit> RecievedItems { get; private set; }

        public void AddRecievedItem(ItemUnit recievedItem)
        {
            RecievedItems.Add(recievedItem);
        }

        public List<RequestedItemUnit> RequestedItems { get; private set; }

        public void AddRequestedItem(RequestedItemUnit requestedItem)
        {
            RequestedItems.Add(requestedItem);
        }
    }
}

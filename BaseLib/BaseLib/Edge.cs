using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BaseLib.Interfaces;

namespace BaseLib
{
    [Serializable]
    public class Edge : IContainsPrize, IRequestItems
    {
        private bool m_IsActive;

        [OptionalField]
        private int m_Priority;

        public Edge(Paragraph @from, Paragraph to, bool isDefault)
        {
            From = @from;
            To = to;
            IsDefault = isDefault;
            IsChanged = !isDefault;
            IsAcite = true;

            RecievedItems = new List<ItemUnit>();
            RequestedItems = new List<RequestedItemUnit>();
        }

        public Paragraph From { get; set; }

        public Paragraph To { get; set; }

        // TODO: prohibit free add

        public bool IsDefault { get; set; }

        public bool IsChanged { get; set; }

        public bool IsAcite
        {
            get { return m_IsActive; }
            set
            {
                m_IsActive = value;
                IsChanged = true;
            }
        }

        public int Priority
        {
            get { return m_Priority; }
            set
            {
                m_Priority = value;
                IsChanged = true;
            }
        }

        #region IContainsPrize Members

        public List<ItemUnit> RecievedItems { get; private set; }

        public void AddRecievedItem(ItemUnit recievedItem)
        {
            RecievedItems.Add(recievedItem);
            IsChanged = true;
        }

        #endregion

        #region IRequestItems Members

        public List<RequestedItemUnit> RequestedItems { get; private set; }

        public void AddRequestedItem(RequestedItemUnit requestedItem)
        {
            RequestedItems.Add(requestedItem);
            IsChanged = true;
        }

        #endregion
    }
}
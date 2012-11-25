using System;
using System.Collections.ObjectModel;
using System.Linq;
using BaseLib;

namespace QuestBookViewModel.Models
{
    public class EdgeModel : ObservableObject
    {
        public EdgeModel(Edge edge)
        {
            m_Edge = edge;

            m_RecievedItems = new ObservableCollection<ItemUnitModel>();
            foreach (var recievedItem in m_Edge.RecievedItems)
            {
                m_RecievedItems.Add(new ItemUnitModel(recievedItem));
            }

            m_RequestedItems =
                new ObservableCollection<RequestedItemUnitModel>(
                    edge.RequestedItems.Select(item => new RequestedItemUnitModel(item)));
        }

        private readonly Edge m_Edge;

        public int FromId
        {
            get { return m_Edge.From.Id; }
        }

        public int ToId
        {
            get { return m_Edge.To.Id; }
        }

        public bool IsActive
        {
            get { return m_Edge.IsAcite; }
            set
            {
                m_Edge.IsAcite = value;
                RaisePropertyChanged("IsActive");
            }
        }

        public bool IsDefault
        {
            get { return m_Edge.IsDefault; }
        }

        public string Priority
        {
            get
            {
                if (m_Edge.Priority == 0)
                {
                    return "";
                }
                return m_Edge.Priority.ToString();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    m_Edge.Priority = 0;
                }
                else
                {
                    m_Edge.Priority = int.Parse(value);
                }
                RaisePropertyChanged("Priority");
            }
        }

        private ObservableCollection<ItemUnitModel> m_RecievedItems;
        public ObservableCollection<ItemUnitModel> RecievedItems
        {
            get { return m_RecievedItems; }
            set
            {
                m_RecievedItems = value;
                RaisePropertyChanged("RecievedItems");
            }
        }

        public void AddRecievedUnit(ItemUnit itemUnit)
        {
            m_Edge.RecievedItems.Add(itemUnit);
            RecievedItems.Add(new ItemUnitModel(itemUnit));
        }

        public void DeleteRecievedUnit(ItemUnitModel itemUnitModel)
        {
            RecievedItems.Remove(itemUnitModel);
            m_Edge.RecievedItems.Remove(itemUnitModel.ItemUnit);
        }

        private ObservableCollection<RequestedItemUnitModel> m_RequestedItems;
        public ObservableCollection<RequestedItemUnitModel> RequestedItems
        {
            get { return m_RequestedItems; }
            set
            {
                m_RequestedItems = value;
                RaisePropertyChanged("RequestedItems");
            }
        }

        public void AddRequestedItem(RequestedItemUnit requestedItem)
        {
            RequestedItems.Add(new RequestedItemUnitModel(requestedItem));
            m_Edge.AddRequestedItem(requestedItem);
        }

        public void RemoveRequetedItem(RequestedItemUnitModel requetedItem)
        {
            RequestedItems.Remove(requetedItem);
            m_Edge.RequestedItems.Remove((RequestedItemUnit) requetedItem.ItemUnit);
        }

        public bool Equals(EdgeModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.m_Edge, m_Edge);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (EdgeModel)) return false;
            return Equals((EdgeModel) obj);
        }

        public override int GetHashCode()
        {
            return (m_Edge != null ? m_Edge.GetHashCode() : 0);
        }
    }
}

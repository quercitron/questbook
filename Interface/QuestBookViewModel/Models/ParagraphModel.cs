using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BaseLib;

namespace QuestBookViewModel.Models
{
    public class ParagraphModel : ObservableObject
    {
        public ParagraphModel(Paragraph paragraph)
        {
            m_Paragraph = paragraph;

            m_RecievedItems = new ObservableCollection<ItemUnitModel>();
            foreach (var recievedItem in m_Paragraph.RecievedItems)
            {
                m_RecievedItems.Add(new ItemUnitModel(recievedItem));
            }

            m_RequestedItems =
                new ObservableCollection<RequestedItemUnitModel>(
                    paragraph.RequestedItems.Select(item => new RequestedItemUnitModel(item)));
        }

        private readonly Paragraph m_Paragraph;

        public int Id
        {
            get { return m_Paragraph.Id; }
        }

        public string Description
        {
            get { return m_Paragraph.Description; }
            set
            {
                m_Paragraph.Description = value;
                RaisePropertyChanged("Description");
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
            m_Paragraph.RecievedItems.Add(itemUnit);
            RecievedItems.Add(new ItemUnitModel(itemUnit));
        }

        public void DeleteRecievedUnit(ItemUnitModel itemUnitModel)
        {
            RecievedItems.Remove(itemUnitModel);
            m_Paragraph.RecievedItems.Remove(itemUnitModel.ItemUnit);
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
            m_Paragraph.AddRequestedItem(requestedItem);
        }

        public void RemoveRequetedItem(RequestedItemUnitModel requetedItem)
        {
            RequestedItems.Remove(requetedItem);
            m_Paragraph.RequestedItems.Remove((RequestedItemUnit)requetedItem.ItemUnit);
        }

        public bool Equals(ParagraphModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.m_Paragraph, m_Paragraph);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ParagraphModel)) return false;
            return Equals((ParagraphModel) obj);
        }

        public override int GetHashCode()
        {
            return (m_Paragraph != null ? m_Paragraph.GetHashCode() : 0);
        }
    }
}

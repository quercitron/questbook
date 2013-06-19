using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using BaseInterfaceLib;
using BaseLib;
using BaseLib.Enumerables;
using BlackWoodBook;
using ModalWindowsService;
using QuestBookViewModel.Models;

namespace QuestBookViewModel
{
    public class QuestBookModel : INotifyPropertyChanged
    {
        public QuestBookModel()
        {
/*            m_Book = new Book("Forest Of Doom", "blackwood.txt", new BlackWoodGraphCreator());*/

            //m_Items = new ObservableCollection<ItemTypeModel>();

            NewItemIsProhibiting = true;

            AddNewItemCommand = new RelayCommand(AddNewItemExecute, AddNewItemCommandCanExecute);
            NewCommand = new RelayCommand(NewCommandExecute);
            LoadCommand = new RelayCommand(LoadCommandExecute);
            SaveCommand = new RelayCommand(SaveCommandExecute, SaveCommandCanExecute);
            DeleteItemCommand = new RelayCommand(DeleteItemCommandExecute, DeleteItemCommandCanExecute);
            AddParagraphRewardItemCommand = new RelayCommand(AddParagraphRewardItemCommandExecute,
                                                             AddParagraphRewardItemCommandCanExecute);
            DeleteParagraphRewardCommand = new RelayCommand(DeleteParagraphRewardCommandExecute, DeleteParagraphRewardCommandCanExecute);
            AddNewEdgeRequestedItemCommand = new RelayCommand(AddNewEdgeRequestedItemCommandExecute,
                                                              AddNewEdgeRequestedItemCommandCanExecute);
            DeleteEdgeRequestedItemCommand = new RelayCommand(DeleteEdgeRequestedItemCommandExecute,
                                                              DeleteEdgeRequestedItemCommandCanExecute);
            GetFurthestWayCommand = new RelayCommand(GetFurthestWayCommandExecute, GetFurthestWayCommandCanExecute);
            ResetParagraphsCommand = new RelayCommand(ResetParagraphsCommandExecute);
            CleanPathCommand = new RelayCommand(CleanPathCommandExecute);
            AddNewParagraphRequestedItemCommand = new RelayCommand(AddNewParagraphRequestedItemCommandExecute,
                                                              AddNewParagraphRequestedItemCommandCanExecute);
            DeleteParagraphRequestedItemCommand = new RelayCommand(DeleteParagraphRequestedItemCommandExecute,
                                                              DeleteParagraphRequestedItemCommandCanExecute);
            AddEdgeRewardItemCommand = new RelayCommand(AddEdgeRewardItemCommandExecute,
                                                             AddEdgeRewardItemCommandCanExecute);
            DeleteEdgeRewardCommand = new RelayCommand(DeleteEdgeRewardCommandExecute, DeleteEdgeRewardCommandCanExecute);
            AddNewEdgeCommand = new RelayCommand(AddNewEdgeCommandExecute, AddNewEdgeCommandCanExecute);

            AddStartItemCommand = new RelayCommand(AddStartItemCommandExecute, AddStartItemCommandCanExecute);

            UpdateCommand = new RelayCommand(UpdateCommandExecute, UpdateCommandCanExecute);
        }

        private void UseBook(Book newBook)
        {
            m_Book = newBook;

            m_Items = new ObservableCollection<ItemTypeModel>(m_Book.AvailableItems.Select(item => new ItemTypeModel(item)));
            NotifyPropertyChanged("Items");

            m_Paragraphs = new ObservableCollection<ParagraphModel>(m_Book.Paragraphs.Select(p => new ParagraphModel(p)));
            NotifyPropertyChanged("Paragraphs");

            if (m_Book.LastSearchParameters != null)
            {
                StartId = m_Book.LastSearchParameters.StartState.ParagraphNo;
                StartItems = new ObservableCollection<ItemUnit>(m_Book.LastSearchParameters.StartState.Items);
                SearchAlgorithm = m_Book.LastSearchParameters.Algorithm;
            }
            else
            {
                StartItems = new ObservableCollection<ItemUnit>();
            }
            NotifyPropertyChanged("StartItems");

            SaveCommand.RaiseCanExecuteChanged();
            GetFurthestWayCommand.RaiseCanExecuteChanged();
            UpdateCommand.RaiseCanExecuteChanged();

            FoundWay = ConvertWay(m_Book.LastGeneratedWay);

            UpdateEdgeList();
        }

        public RelayCommand AddNewItemCommand { get; private set; }

        private bool AddNewItemCommandCanExecute()
        {
            return !(string.IsNullOrEmpty(NewItemName) || m_Book == null ||
                     Items.Any(item => item.Name.Equals(NewItemName, StringComparison.InvariantCultureIgnoreCase)));
        }

        private void AddNewItemExecute()
        {
            if (!string.IsNullOrEmpty(NewItemName))
            {
                // TODO: Change constructor
                var newItem = new ItemType(NewItemName, NewItemIsUnique, NewItemIsVital);
                m_Book.AvailableItems.Add(newItem);
                Items.Add(new ItemTypeModel(newItem));

                AddNewItemCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand NewCommand { get; private set; }

        private void NewCommandExecute()
        {
            var service = new OpenBookService();
            service.BookSelected += CreateBook;
            service.GetBook();
        }

        private void CreateBook(object sender, SelectedBookArgs e)
        {
            var newBook = Book.CreateBook(e.FilePath, e.BookType);
            UseBook(newBook);
        }

        public RelayCommand LoadCommand { get; private set; }

        private void LoadCommandExecute()
        {
            var newBook = Book.Load("Braslavskiy_Tayna_kapitana_Sheltona.fb2.qbs");
            UseBook(newBook);
        }

        public RelayCommand SaveCommand { get; private set; }

        private void SaveCommandExecute()
        {
            m_Book.Save();
        }

        private bool SaveCommandCanExecute()
        {
            return m_Book != null;
        }

        public RelayCommand DeleteItemCommand { get; private set; }

        private void DeleteItemCommandExecute()
        {
            if (MessageBox.Show(string.Format("Delete Item '{0}'?", m_SelectedItem), "Confirmation", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
            {
                return;
            }

            m_Book.RemoveItem(m_SelectedItem.ItemType);
            RemoveItem(m_SelectedItem);
        }

        public RelayCommand UpdateCommand { get; private set; }

        public bool UpdateCommandCanExecute()
        {
            return m_Book != null;
        }

        public void UpdateCommandExecute()
        {
            // TODO: Change
            m_Book.Update("BlackWood.txt");            
        }

        private void RemoveItem(ItemTypeModel itemTypeModel)
        {
            foreach (var paragraph in Paragraphs)
            {
                var toRemove = paragraph.RecievedItems.Where(item => item.BasicItem.Equals(itemTypeModel)).ToList();
                foreach (var item in toRemove)
                {
                    paragraph.RecievedItems.Remove(item);
                }
            }            

            foreach (var edge in Edges)
            {
                // TODO: Finish
                //edge.RecievedItems.RemoveAll(item => item.BasicItem == itemTypeModel);
                //edge.RequestedItems.RemoveAll(item => item.BasicItem == itemTypeModel);
            }

            Items.Remove(itemTypeModel);
        }

        private bool DeleteItemCommandCanExecute()
        {
            return m_SelectedItem != null;
        }

        private Book m_Book;

        private ObservableCollection<ItemTypeModel> m_Items;
        public ObservableCollection<ItemTypeModel> Items
        {
            get { return m_Items; }
        }

        private ObservableCollection<ParagraphModel> m_Paragraphs;
        public ObservableCollection<ParagraphModel> Paragraphs
        {
            get { return m_Paragraphs; }
        }

        private string m_NewItemName;
        public string NewItemName
        {
            get { return m_NewItemName; }
            set
            {
                if (m_NewItemName != value)
                {
                    m_NewItemName = value;
                    NotifyPropertyChanged("NewItemName");
                    AddNewItemCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private bool m_NewItemIsUnique;
        public bool NewItemIsUnique
        {
            get { return m_NewItemIsUnique; }
            set
            {
                if (m_NewItemIsUnique != value)
                {
                    m_NewItemIsUnique = value;
                    NotifyPropertyChanged("NewItemIsUnique");
                }
            }
        }

        private bool m_NewItemIsProhibiting;
        public bool NewItemIsProhibiting
        {
            get { return m_NewItemIsProhibiting; }
            set
            {
                if (m_NewItemIsProhibiting != value)
                {
                    m_NewItemIsProhibiting = value;
                    NotifyPropertyChanged("NewItemIsProhibiting");
                }
            }
        }

        private bool m_NewItemIsVital;
        public bool NewItemIsVital
        {
            get { return m_NewItemIsVital; }
            set
            {
                if (m_NewItemIsVital != value)
                {
                    m_NewItemIsVital = value;
                    NotifyPropertyChanged("NewItemIsVital");
                }
            }
        }

        private ItemTypeModel m_SelectedItem;
        public ItemTypeModel SelectedItem
        {
            set
            {
                m_SelectedItem = value;
                DeleteItemCommand.RaiseCanExecuteChanged();
            }
        }

        private ParagraphModel m_SelectedParagraph;
        public ParagraphModel SelectedParagraph
        {
            get { return m_SelectedParagraph; }
            set
            {
                m_SelectedParagraph = value;
                NotifyPropertyChanged("SelectedParagraph");
                AddParagraphRewardItemCommand.RaiseCanExecuteChanged();
            }
        }

        private ItemTypeModel m_NewRewardParagraphItemType;
        public ItemTypeModel NewRewardParagraphItemType
        {
            get { return m_NewRewardParagraphItemType; }
            set
            {
                m_NewRewardParagraphItemType = value;
                NotifyPropertyChanged("NewRewardParagraphItemType");
                AddParagraphRewardItemCommand.RaiseCanExecuteChanged();
            }
        }

        private int m_NewRewardParagraphItemTypeCount;
        public int NewRewardParagraphItemTypeCount
        {
            get { return m_NewRewardParagraphItemTypeCount; }
            set
            {
                m_NewRewardParagraphItemTypeCount = value;
                AddParagraphRewardItemCommand.RaiseCanExecuteChanged();
                NotifyPropertyChanged("NewRewardParagraphItemTypeCount");
            }
        }

        public RelayCommand AddParagraphRewardItemCommand { get; set; }

        private void AddParagraphRewardItemCommandExecute()
        {
            var itemUnit = new ItemUnit(NewRewardParagraphItemType.ItemType, NewRewardParagraphItemTypeCount);
            SelectedParagraph.AddRecievedUnit(itemUnit);
            AddParagraphRewardItemCommand.RaiseCanExecuteChanged();
        }

        private bool AddParagraphRewardItemCommandCanExecute()
        {
            return SelectedParagraph != null && NewRewardParagraphItemType != null &&
                   SelectedParagraph.RecievedItems.All(item => item.BasicItem.ItemType != NewRewardParagraphItemType.ItemType) &&
                   NewRewardParagraphItemTypeCount > 0;
        }

        private ItemUnitModel m_SelectedParagraphReward;
        public ItemUnitModel SelectedParagraphReward
        {
            get { return m_SelectedParagraphReward; }
            set
            {
                m_SelectedParagraphReward = value;
                NotifyPropertyChanged("SelectedParagraphReward");
                DeleteParagraphRewardCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand DeleteParagraphRewardCommand { get; set; }

        private void DeleteParagraphRewardCommandExecute()
        {
            if (MessageBox.Show(string.Format("Delete reward '{0}'?", SelectedParagraphReward.BasicItem.Name) , "Confirmation", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
            {
                return;
            }

            SelectedParagraph.DeleteRecievedUnit(SelectedParagraphReward);
        }

        private bool DeleteParagraphRewardCommandCanExecute()
        {
            return SelectedParagraph != null;
        }

        private int? m_FromId;
        public int? FromId
        {
            get { return m_FromId; }
            set
            {
                m_FromId = value;
                NotifyPropertyChanged("FromId");
                UpdateEdgeList();
                AddNewEdgeCommand.RaiseCanExecuteChanged();
            }
        }

        private int? m_ToId;
        public int? ToId
        {
            get { return m_ToId; }
            set
            {
                m_ToId = value;
                NotifyPropertyChanged("ToId");
                UpdateEdgeList();
                AddNewEdgeCommand.RaiseCanExecuteChanged();
            }
        }

        private void UpdateEdgeList()
        {
            if (m_Book != null)
            {
                Edges = new ObservableCollection<EdgeModel>(m_Book.GetEdges(FromId, ToId).Select(e => new EdgeModel(e)));
                // TODO: Duplicated update?
                SelectedEdge = null;
                NotifyPropertyChanged("Edges");
            }
        }

        private ObservableCollection<EdgeModel> m_Edges;
        public ObservableCollection<EdgeModel> Edges
        {
            get { return m_Edges; }
            set
            {
                m_Edges = value;
                NotifyPropertyChanged("Edges");
            }
        }

        private ItemTypeModel m_NewEdgeRequestedItemType;
        public ItemTypeModel NewEdgeRequestedItemType
        {
            get { return m_NewEdgeRequestedItemType; }
            set
            {
                m_NewEdgeRequestedItemType = value;
                NotifyPropertyChanged("NewEdgeRequestedItemType");
                AddNewEdgeRequestedItemCommand.RaiseCanExecuteChanged();

                // TODO: Check or remake
                if (m_NewEdgeRequestedItemType.IsUnique)
                {
                    NewEdgeRequestedItemCount = 1;
                }
                else
                {
                    NewEdgeRequestedItemIsDisappearing = true;
                }
            }
        }

        private int m_NewEdgeRequestedItemCount;
        public int NewEdgeRequestedItemCount
        {
            get { return m_NewEdgeRequestedItemCount; }
            set
            {
                m_NewEdgeRequestedItemCount = value;
                NotifyPropertyChanged("NewEdgeRequestedItemCount");
                AddNewEdgeRequestedItemCommand.RaiseCanExecuteChanged();
            }
        }

        private bool m_NewEdgeRequestedItemIsDisappearing;
        public bool NewEdgeRequestedItemIsDisappearing
        {
            get { return m_NewEdgeRequestedItemIsDisappearing; }
            set
            {
                m_NewEdgeRequestedItemIsDisappearing = value;
                NotifyPropertyChanged("NewEdgeRequestedItemIsDisappearing");
                AddNewEdgeRequestedItemCommand.RaiseCanExecuteChanged();
            }
        }

        private EdgeModel m_SelectedEdge;

        public EdgeModel SelectedEdge
        {
            get { return m_SelectedEdge; }
            set
            {
                m_SelectedEdge = value;
                AddNewEdgeRequestedItemCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand AddNewEdgeRequestedItemCommand { get; private set; }

        private void AddNewEdgeRequestedItemCommandExecute()
        {
            var requestedItem = new RequestedItemUnit(NewEdgeRequestedItemType.ItemType, NewEdgeRequestedItemCount,
                                                      NewEdgeRequestedItemIsDisappearing);
            SelectedEdge.AddRequestedItem(requestedItem);
        }

        private bool AddNewEdgeRequestedItemCommandCanExecute()
        {
            // TODO: Reject new item adding
            return SelectedEdge != null && m_NewEdgeRequestedItemType != null && NewEdgeRequestedItemCount != 0;
        }

        private RequestedItemUnitModel m_SelectedEdgeRequetedItem;
        public RequestedItemUnitModel SelectedEdgeRequetedItem
        {
            get { return m_SelectedEdgeRequetedItem; }
            set
            {
                m_SelectedEdgeRequetedItem = value;
                NotifyPropertyChanged("SelectedEdgeRequetedItem");
                DeleteEdgeRequestedItemCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand DeleteEdgeRequestedItemCommand { get; private set; }

        private void DeleteEdgeRequestedItemCommandExecute()
        {
            SelectedEdge.RemoveRequetedItem(SelectedEdgeRequetedItem);
            DeleteEdgeRequestedItemCommand.RaiseCanExecuteChanged();
        }

        private bool DeleteEdgeRequestedItemCommandCanExecute()
        {
            return SelectedEdgeRequetedItem != null;
        }

        //----------------------------------------------------------
        private ItemTypeModel m_NewParagraphRequestedItemType;
        public ItemTypeModel NewParagraphRequestedItemType
        {
            get { return m_NewParagraphRequestedItemType; }
            set
            {
                m_NewParagraphRequestedItemType = value;
                NotifyPropertyChanged("NewParagraphRequestedItemType");
                AddNewParagraphRequestedItemCommand.RaiseCanExecuteChanged();
            }
        }

        private int m_NewParagraphRequestedItemCount;
        public int NewParagraphRequestedItemCount
        {
            get { return m_NewParagraphRequestedItemCount; }
            set
            {
                m_NewParagraphRequestedItemCount = value;
                NotifyPropertyChanged("NewParagraphRequestedItemCount");
                AddNewParagraphRequestedItemCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand AddNewParagraphRequestedItemCommand { get; private set; }

        private void AddNewParagraphRequestedItemCommandExecute()
        {
            var requestedItem = new RequestedItemUnit(NewParagraphRequestedItemType.ItemType,
                                                      NewParagraphRequestedItemCount,
                                                      true);
            SelectedParagraph.AddRequestedItem(requestedItem);
        }

        private bool AddNewParagraphRequestedItemCommandCanExecute()
        {
            // TODO: Reject new item adding
            return SelectedParagraph != null && m_NewParagraphRequestedItemType != null && NewParagraphRequestedItemCount > 0;
        }

        private RequestedItemUnitModel m_SelectedParagraphRequetedItem;
        public RequestedItemUnitModel SelectedParagraphRequetedItem
        {
            get { return m_SelectedParagraphRequetedItem; }
            set
            {
                m_SelectedParagraphRequetedItem = value;
                NotifyPropertyChanged("SelectedParagraphRequetedItem");
                DeleteParagraphRequestedItemCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand DeleteParagraphRequestedItemCommand { get; private set; }

        private void DeleteParagraphRequestedItemCommandExecute()
        {
            SelectedParagraph.RemoveRequetedItem(SelectedParagraphRequetedItem);
            DeleteParagraphRequestedItemCommand.RaiseCanExecuteChanged();
        }

        private bool DeleteParagraphRequestedItemCommandCanExecute()
        {
            return SelectedParagraphRequetedItem != null;
        }
        //----------------------------------------------------------

        //-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|

        private ItemTypeModel m_NewRewardEdgeItemType;
        public ItemTypeModel NewRewardEdgeItemType
        {
            get { return m_NewRewardEdgeItemType; }
            set
            {
                m_NewRewardEdgeItemType = value;
                NotifyPropertyChanged("NewRewardEdgeItemType");
                AddEdgeRewardItemCommand.RaiseCanExecuteChanged();
            }
        }

        private int m_NewRewardEdgeItemTypeCount;
        public int NewRewardEdgeItemTypeCount
        {
            get { return m_NewRewardEdgeItemTypeCount; }
            set
            {
                m_NewRewardEdgeItemTypeCount = value;
                AddEdgeRewardItemCommand.RaiseCanExecuteChanged();
                NotifyPropertyChanged("NewRewardEdgeItemTypeCount");
            }
        }

        public RelayCommand AddEdgeRewardItemCommand { get; set; }

        private void AddEdgeRewardItemCommandExecute()
        {
            var itemUnit = new ItemUnit(NewRewardEdgeItemType.ItemType, NewRewardEdgeItemTypeCount);
            SelectedEdge.AddRecievedUnit(itemUnit);
            AddEdgeRewardItemCommand.RaiseCanExecuteChanged();
        }

        private bool AddEdgeRewardItemCommandCanExecute()
        {
            return SelectedEdge != null && NewRewardEdgeItemType != null &&
                   SelectedEdge.RecievedItems.All(item => item.BasicItem.ItemType != NewRewardEdgeItemType.ItemType) &&
                   NewRewardEdgeItemTypeCount > 0;
        }

        private ItemUnitModel m_SelectedEdgeReward;
        public ItemUnitModel SelectedEdgeReward
        {
            get { return m_SelectedEdgeReward; }
            set
            {
                m_SelectedEdgeReward = value;
                NotifyPropertyChanged("SelectedEdgeReward");
                DeleteEdgeRewardCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand DeleteEdgeRewardCommand { get; set; }

        private void DeleteEdgeRewardCommandExecute()
        {
            if (MessageBox.Show(string.Format("Delete reward '{0}'?", SelectedEdgeReward.BasicItem.Name), "Confirmation", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
            {
                return;
            }

            SelectedEdge.DeleteRecievedUnit(SelectedEdgeReward);
        }

        private bool DeleteEdgeRewardCommandCanExecute()
        {
            return SelectedEdge != null;
        }

        //-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|

        private List<SearchResultStateModel> m_FoundWay;
        public List<SearchResultStateModel> FoundWay
        {
            get { return m_FoundWay; }
            set
            {
                m_FoundWay = value;
                NotifyPropertyChanged("FoundWay");
            }
        }

        public RelayCommand CleanPathCommand { get; private set; }

        private void CleanPathCommandExecute()
        {
            var result = MessageBox.Show("Are you sure you want to Clean current path?", "Clean", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
            {
                return;
            }
            // TODO: move into book
            foreach (var step in m_Book.LastGeneratedWay)
            {
                if (step.VisitedFirstTime)
                {
                    m_Book.GetParagraph(step.State.ParagraphNo).WasVisited = false;
                }
            }
        }

        public RelayCommand ResetParagraphsCommand { get; private set; }

        private void ResetParagraphsCommandExecute()
        {
            var result = MessageBox.Show("Are you sure you want to Reset all paragraphs?", "Reset", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            // TODO: move into book
            foreach (var paragraph in m_Book.Paragraphs)
            {
                paragraph.WasVisited = false;
            }
        }

        public RelayCommand GetFurthestWayCommand { get; private set; }

        private void GetFurthestWayCommandExecute()
        {
            var searchParameters = new SearchParameters(new PersonState(StartId, StartItems.ToList()), SearchAlgorithm.Value);

            List<SearchResultState> newWay = null;

            // TODO: Change to application start settings
            Thread thread = new Thread(() => newWay = m_Book.GetWay(searchParameters), 256 * 1024 * 1024);

            ComputingRunning = true;

            thread.Start();
            thread.Join();

            ComputingRunning = false;

            FoundWay = ConvertWay(newWay);
        }

        private bool m_ComputingRunning;

        public bool ComputingRunning
        {
            get { return m_ComputingRunning; }
            private set
            {
                m_ComputingRunning = value;
                NotifyPropertyChanged("ComputingRunning");
            }
        }

        private bool GetFurthestWayCommandCanExecute()
        {
            return m_Book != null && Paragraphs.Any(p => p.Id == StartId) && SearchAlgorithm != null;
        }

        private List<SearchResultStateModel> ConvertWay(IEnumerable<SearchResultState> baseWay)
        {
            List<SearchResultStateModel> newWay = null;

            if (baseWay != null)
            {
                newWay = baseWay.Select(s => new SearchResultStateModel(s)).ToList();
            }

            return newWay;
        }        

        private SearchResultStateModel m_SelectedResultState;
        public SearchResultStateModel SelectedResultState
        {
            get { return m_SelectedResultState; }
            set
            {
                m_SelectedResultState = value;
                NotifyPropertyChanged("SelectedResultState");
                if (m_SelectedResultState != null)
                {
                    SelectedParagraph = Paragraphs.First(p => p.Id == m_SelectedResultState.State.ParagraphNo);
                    FromId = m_SelectedResultState.State.ParagraphNo;
                    ToId = null;
                }
            }
        }

        /*private int? m_NewEdgeFrom;
        public int? NewEdgeFrom
        {
            get { return m_NewEdgeFrom; }
            set
            {
                m_NewEdgeFrom = value;
                NotifyPropertyChanged("NewEdgeFrom");
                AddNewEdgeCommand.RaiseCanExecuteChanged();
            }
        }

        private int? m_NewEdgeTo;
        public int? NewEdgeTo
        {
            get { return m_NewEdgeTo; }
            set
            {
                m_NewEdgeTo = value;
                NotifyPropertyChanged("NewEdgeTo");
                AddNewEdgeCommand.RaiseCanExecuteChanged();
            }
        }*/

        public RelayCommand AddNewEdgeCommand { get; private set; }

        private void AddNewEdgeCommandExecute()
        {
            if (FromId.HasValue && ToId.HasValue)
            {
                m_Book.AddNewEdge(FromId.Value, ToId.Value);
                UpdateEdgeList();
                /*FromId = NewEdgeFrom.Value;
                ToId = NewEdgeTo.Value;*/
            }
        }

        private bool AddNewEdgeCommandCanExecute()
        {
            return m_Book != null && FromId.HasValue && ToId.HasValue;
        }

        // Start Parameters

        public RelayCommand AddStartItemCommand { get; private set; }

        private void AddStartItemCommandExecute()
        {
            StartItems.Add(new ItemUnit(NewStartItemType.ItemType, NewStartItemCount.Value));
        }

        private bool AddStartItemCommandCanExecute()
        {
            return NewStartItemType != null && NewStartItemCount.HasValue;
        }

        private SearchAlgorithm? m_SearchAlgorithm;
        public SearchAlgorithm? SearchAlgorithm
        {
            get { return m_SearchAlgorithm; }
            set
            {
                m_SearchAlgorithm = value;
                NotifyPropertyChanged("SearchAlgorithm");
                GetFurthestWayCommand.RaiseCanExecuteChanged();
            }
        }

        private int m_StartId;
        public int StartId
        {
            get { return m_StartId; }
            set
            {
                m_StartId = value;
                NotifyPropertyChanged("StartId");
                GetFurthestWayCommand.RaiseCanExecuteChanged();
            }
        }

        private ItemTypeModel m_NewStartItemType;
        public ItemTypeModel NewStartItemType
        {
            get { return m_NewStartItemType; }
            set
            {
                m_NewStartItemType = value;
                NotifyPropertyChanged("NewStartItemType");
                AddStartItemCommand.RaiseCanExecuteChanged();
            }
        }

        private int? m_NewStartItemCount;
        public int? NewStartItemCount
        {
            get { return m_NewStartItemCount; }
            set
            {
                m_NewStartItemCount = value;
                NotifyPropertyChanged("NewStartItemCount");
                AddStartItemCommand.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<ItemUnit> StartItems { get; private set; }

        // Event

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}

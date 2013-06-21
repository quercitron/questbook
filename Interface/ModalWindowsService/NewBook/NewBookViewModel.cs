using System.ComponentModel;
using System.Windows.Forms;
using BaseInterfaceLib;
using GraphCreatorInterface;

namespace ModalWindowsService
{
    public class NewBookViewModel : INotifyPropertyChanged
    {
        private readonly NewBookService m_Service;

        private string m_Path;

        public string Path
        {
            get { return m_Path; }
            set
            {
                m_Path = value;
                OnPropertyChanged("Path");
            }
        }

        public QuestBookType BookType { get; set; }

        public RelayCommand BrowseCommand { get; set; }

        public RelayCommand CreateCommand { get; set; }

        private void BrowseCommandExecute()
        {
            var dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                Path = dialog.FileName;
            }
        }

        private void CreateCommandExecute()
        {
            // TODO: change order?
            m_Service.NotifyBookSelected(Path, BookType);
            m_Service.Window.Close();
        }

        public NewBookViewModel(NewBookService service)
        {
            m_Service = service;
            BrowseCommand = new RelayCommand(BrowseCommandExecute);
            CreateCommand = new RelayCommand(CreateCommandExecute);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
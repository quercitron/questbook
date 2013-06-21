using System.ComponentModel;
using System.Windows.Forms;
using BaseInterfaceLib;

namespace ModalWindowsService.SelectFile
{
    public class SelectFileViewModel : INotifyPropertyChanged
    {
        private readonly SelectFileService m_Service;

        private readonly string m_Extensions;
        private readonly string m_DefaultExtensions = "Quest Book Saves|*.qbs";
        private readonly string m_AllFilesEnd = "|All Files (*.*)|*.*";

        private string m_FilePath;

        public string FilePath
        {
            get { return m_FilePath; }
            set
            {
                m_FilePath = value;
                OnPropertyChanged("FilePath");
            }
        }

        public RelayCommand BrowseCommand { get; set; }

        public RelayCommand OpenCommand { get; set; }

        private void BrowseCommandExecute()
        {
            var dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = this.m_Extensions;
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                FilePath = dialog.FileName;
            }
        }

        private void OpenCommandExecute()
        {
            // TODO: change order?
            m_Service.NotifyFileSelected(FilePath);
            m_Service.Window.Close();
        }

        public SelectFileViewModel(SelectFileService service, string extensions = null)
        {
            m_Service = service;
            m_Extensions = (extensions ?? m_DefaultExtensions) + m_AllFilesEnd;
            BrowseCommand = new RelayCommand(BrowseCommandExecute);
            OpenCommand = new RelayCommand(OpenCommandExecute);
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

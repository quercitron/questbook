using System.Windows;
using QuestBookViewModel;

namespace QuestBookApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly QuestBookModel m_ViewModel;

        public MainWindow()
        {
            InitializeComponent();

            m_ViewModel = new QuestBookModel();

            DataContext = m_ViewModel;
        }
    }
}

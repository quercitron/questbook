using System;
using System.Windows;
using GraphCreatorInterface;

namespace ModalWindowsService
{
    public class OpenBookService
    {
        public void GetBook()
        {
            Window = new OpenBookWindow();
            Window.DataContext = new OpenBookViewModel(this);
            Window.Show();
        }

        public Window Window { get; set; }

        public event EventHandler<SelectedBookArgs> BookSelected;

        public void NotifyBookSelected(string path, QuestBookType bookType)
        {
            var handler = BookSelected;
            if (handler != null)
            {
                handler(this, new SelectedBookArgs(path, bookType));
            }
        }
    }
}

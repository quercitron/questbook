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

    public class SelectedBookArgs : EventArgs
    {
        public SelectedBookArgs(string path, QuestBookType bookType)
        {
            FilePath = path;
            BookType = bookType;
        }

        public string FilePath { get; private set; }

        public QuestBookType BookType { get; private set; }
    }
}

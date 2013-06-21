using System;
using System.Windows;

using GraphCreatorInterface;

namespace ModalWindowsService.NewBook
{
    public class NewBookService : INewBookService
    {
        public void GetBook()
        {
            Window = new NewBookWindow();
            Window.DataContext = new NewBookViewModel(this);
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

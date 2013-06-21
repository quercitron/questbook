using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ModalWindowsService.SelectFile
{
    public class SelectFileService : ISelectFileService
    {
        public Window Window { get; private set; }

        public void SelectFile(string title)
        {
            Window = new SelectFileWindow {Title = title};
            Window.DataContext = new SelectFileViewModel(this);
            Window.Show();
        }

        public void NotifyFileSelected(string path)
        {
            var handler = BookSelected;
            if (handler != null)
            {
                handler(this, new SelectedFleArgs {FilePath = path});
            }
        }

        public event EventHandler<SelectedFleArgs> BookSelected;
    }
}

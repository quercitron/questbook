using System;

namespace ModalWindowsService.NewBook
{
    public interface INewBookService
    {
        void GetBook();

        event EventHandler<SelectedBookArgs> BookSelected;
    }
}
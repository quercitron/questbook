using System;

namespace ModalWindowsService
{
    public interface INewBookService
    {
        void GetBook();

        event EventHandler<SelectedBookArgs> BookSelected;
    }
}
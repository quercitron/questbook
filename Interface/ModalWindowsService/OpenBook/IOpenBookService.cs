using System;

namespace ModalWindowsService
{
    public interface IOpenBookService
    {
        void GetBook();

        event EventHandler<SelectedBookArgs> BookSelected;
    }
}
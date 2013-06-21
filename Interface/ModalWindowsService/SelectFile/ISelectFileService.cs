using System;

namespace ModalWindowsService.SelectFile
{
    public interface ISelectFileService
    {
        void SelectFile(string title);

        event EventHandler<SelectedFleArgs> BookSelected;
    }
}

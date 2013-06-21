using System;

namespace ModalWindowsService.SelectFile
{
    public interface ISelectFileService
    {
        void SelectFile(SelectFileParameters parameters);

        event EventHandler<SelectedFleArgs> BookSelected;
    }
}

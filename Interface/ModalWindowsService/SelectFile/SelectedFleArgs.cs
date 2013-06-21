using System;

namespace ModalWindowsService.SelectFile
{
    public class SelectedFleArgs : EventArgs
    {
        public string FilePath { get; set; }
    }
}
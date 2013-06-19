using System;
using GraphCreatorInterface;

namespace ModalWindowsService
{
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
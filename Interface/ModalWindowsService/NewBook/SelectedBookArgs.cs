using System;

using GraphCreatorInterface;

namespace ModalWindowsService.NewBook
{
    public class SelectedBookArgs : EventArgs
    {
        public SelectedBookArgs(string path, QuestBookType bookType)
        {
            this.FilePath = path;
            this.BookType = bookType;
        }

        public string FilePath { get; private set; }

        public QuestBookType BookType { get; private set; }
    }
}
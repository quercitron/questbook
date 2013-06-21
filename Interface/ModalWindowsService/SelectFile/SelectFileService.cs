using System;
using System.Windows.Forms;

namespace ModalWindowsService.SelectFile
{
    public class SelectFileService : ISelectFileService
    {
        private readonly string m_DefaultExtensions = "Quest Book Saves|*.qbs";
        private readonly string m_AllFilesEnd = "|All Files (*.*)|*.*";

        public void SelectFile(SelectFileParameters parameters)
        {
            FileDialog dialog;
            if (parameters.SaveFile)
            {
                dialog = new SaveFileDialog
                    {
                        Filter = (parameters.Extensions ?? this.m_DefaultExtensions) + this.m_AllFilesEnd,
                        FileName = parameters.DefaultName
                    };
            }
            else
            {
                dialog = new OpenFileDialog
                    {
                        Multiselect = false,
                        Filter = (parameters.Extensions ?? this.m_DefaultExtensions) + this.m_AllFilesEnd
                    };
            }
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                NotifyFileSelected(dialog.FileName);
            }
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

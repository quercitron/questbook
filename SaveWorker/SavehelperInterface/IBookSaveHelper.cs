using BaseLib;

namespace SavehelperInterface
{
    public interface IBookSaveHelper
    {
        void Save(Book book, string path);

        Book Load(string path);
    }
}

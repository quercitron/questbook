using System.IO;
using System.Xml.Serialization;
using BaseLib;
using SavehelperInterface;

namespace SaveWorker
{
    public class SaveHelper : IBookSaveHelper
    {
        public void Save(Book book, string path)
        {
            var serializer = new XmlSerializer(typeof (Book));
            using (TextWriter writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, this);
            }
        }

        public Book Load(string path)
        {
            var serializer = new XmlSerializer(typeof (Book));
            using (TextReader reader = new StringReader(path))
            {
                return (Book) serializer.Deserialize(reader);
            }
        }
    }
}

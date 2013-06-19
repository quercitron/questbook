using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GraphCreatorInterface;

namespace CaptainSheltonBook
{
    [Serializable]
    public class CaptainSheltonGraphCreator : BaseGraphCreator
    {
        public override BaseGraph CreateGraphFromText(string text)
        {
            var graph = new BaseGraph();

            var stream = new MemoryStream(Encoding.Default.GetBytes(text));
            XElement book = XElement.Load(stream);

            foreach (var element in book.Elements("body").Elements("section").Where(s => s.HasAttributes))
            {
                var currentParagraph = Convert.ToInt32(element.Element("title").Element("p").Value);
                var paths = element.Elements("p").Elements("a").Select(path => Convert.ToInt32(path.Value.Trim('(', ')')));
                foreach (var path in paths)
                {
                    graph.AddEdge(currentParagraph, path);
                }
                var description = string.Join(Environment.NewLine, element.Elements("p").Select(p => p.Value));
                graph.Descriptions.Add(currentParagraph, description);
            }
            return graph;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphCreatorInterface
{
    public class BaseGraph
    {
        public BaseGraph()
        {
            Vertices = new HashSet<int>();
            Edges = new List<SimpleEdge>();
            Descriptions = new Dictionary<int, string>();
        }

        public HashSet<int> Vertices { get; private set; }

        public List<SimpleEdge> Edges { get; private set; }

        public Dictionary<int, string> Descriptions { get; private set; } 

        public void AddEdge(int from, int to)
        {
            Edges.Add(new SimpleEdge(from, to));
            Vertices.Add(from);
            Vertices.Add(to);
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(string.Format("Vertices count: {0}", Vertices.Count));
            foreach (var vertex in Vertices.OrderBy(v => v))
            {
                stringBuilder.Append(string.Format("{0, 3} :", vertex));
                foreach (var edge in Edges.Where(e => e.From == vertex))
                {
                    stringBuilder.Append(string.Format(" {0}", edge.To));
                }
                stringBuilder.AppendLine();
            }
            return stringBuilder.ToString();
        }
    }
}

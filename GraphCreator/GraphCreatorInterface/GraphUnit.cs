using System.Collections.Generic;

namespace GraphCreatorInterface
{
    public class GraphUnit
    {
        public GraphUnit(int id)
        {
            Id = id;
            Edges = new List<int>();
        }

        public int Id { get; set; }
        public List<int> Edges { get; private set; }
    }
}

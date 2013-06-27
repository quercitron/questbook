namespace FastSearch
{
    public class Edge : Event
    {
        public int Priority { get; set; }

        public Paragraph Start { get; set; }

        public Paragraph End { get; set; }
    }
}

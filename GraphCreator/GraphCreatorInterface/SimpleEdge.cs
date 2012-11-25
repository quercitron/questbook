namespace GraphCreatorInterface
{
    public class SimpleEdge
    {
        public SimpleEdge(int @from, int to)
        {
            this.From = @from;
            this.To = to;
        }

        public int From { get; set; }

        public int To { get; set; }
    }
}

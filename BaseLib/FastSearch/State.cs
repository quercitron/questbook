using System;

namespace FastSearch
{
    public class State
    {
        public int Id { get; set; }

        public int ParagraphNo { get; set; }

        public Int64 Mask { get; set; }

        public State PrevState { get; set; }
    }
}

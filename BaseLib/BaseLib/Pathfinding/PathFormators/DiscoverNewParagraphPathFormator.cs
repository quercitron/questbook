using System.Collections.Generic;
using System.Linq;

namespace BaseLib.Pathfinding.PathFormators
{
    public class DiscoverNewParagraphPathFormator : BasePathFormator
    {
        public DiscoverNewParagraphPathFormator(Dictionary<int, Paragraph> paragraphs)
        {
            m_Paragraphs = paragraphs;
        }

        private readonly Dictionary<int, Paragraph> m_Paragraphs;

        public override List<SearchResultState> FormPath(Dictionary<PersonState, int> states)
        {
            var finalState = states.OrderByDescending(p => p.Value).Select(p => p.Key).FirstOrDefault(s => !m_Paragraphs[s.ParagraphNo].WasVisited);
            if (finalState != null)
            {
                return PathToState(states, finalState);
            }
            return null;
        }
    }
}

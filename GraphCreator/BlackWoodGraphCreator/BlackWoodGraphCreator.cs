using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphCreatorInterface;

namespace BlackWoodBook
{
    [Serializable]
    public class BlackWoodGraphCreator : BaseGraphCreator
    {
        public override BaseGraph CreateGraphFromText(string text)
        {
            var result = new BaseGraph();

            var split = text.ToLower().Split();
            int currentParagraph = 0;
            int state = 0; // 0 -- base state, nothing found
            var description = new StringBuilder();

            string last = null;

            foreach (var part in split)
            {
                var str = RemoveSpecialCharacters(part);

                if (last != null)
                {
                    description.AppendFormat("{0} ", last);
                }
                last = part;

                if (str == "иди")
                {
                    state = 1; // 1 -- "иди" was found
                    continue;
                }

                if (state == 1 && str == "на")
                {
                    state = 2; // 2 -- "на" was found
                    continue;
                }

                int num;
                if (int.TryParse(str, out num))
                {
                    if (state == 2)
                    {
                        if (num != currentParagraph)
                        {
                            result.AddEdge(currentParagraph, num);
                        }
                        state = 0;
                        continue;
                    }

                    if (num == currentParagraph + 1)
                    {
                        result.Descriptions.Add(currentParagraph, description.ToString());
                        description.Clear();
                        last = null;

                        currentParagraph++;
                        state = 0;
                        continue;
                    }
                }

                state = 0;
            }

            result.Descriptions.Add(currentParagraph, description.ToString());

            return result;
        }

        private string RemoveSpecialCharacters(string text)
        {
            var builder = new StringBuilder();
            foreach (var ch in text.Where(Char.IsLetterOrDigit))
            {
                builder.Append(ch);
            }
            return builder.ToString();
        }
    }
}

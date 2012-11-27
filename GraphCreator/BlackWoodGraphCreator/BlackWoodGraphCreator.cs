﻿using System;
using System.Linq;
using System.Text;

using GraphCreatorInterface;

namespace BlackWoodBook
{
    [Serializable]
    public class BlackWoodGraphCreator : BaseGraphCreator
    {
        #region Public Methods and Operators

        public override BaseGraph CreateGraphFromText(string text)
        {
            var result = new BaseGraph();

            string[] split = text.ToLower().Split();
            int currentParagraph = 0;
            int state = 0; // 0 -- base state, nothing found

            int currentDescriptionPosition = 0;

            int start;
            string description;

            foreach (string part in split)
            {
                string str = this.RemoveSpecialCharacters(part);

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
                        start = text.IndexOf(currentParagraph.ToString(), currentDescriptionPosition);
                        int end = text.IndexOf((currentParagraph + 1).ToString(), start);

                        description = text.Substring(start, end - start);
                        result.Descriptions.Add(currentParagraph, description);

                        currentDescriptionPosition = end;

                        currentParagraph++;
                        state = 0;
                        continue;
                    }
                }

                state = 0;
            }

            start = text.IndexOf(currentParagraph.ToString(), currentDescriptionPosition);
            description = text.Substring(start);
            result.Descriptions.Add(currentParagraph, description);

            return result;
        }

        #endregion

        #region Methods

        private string RemoveSpecialCharacters(string text)
        {
            var builder = new StringBuilder();
            foreach (char ch in text.Where(Char.IsLetterOrDigit))
            {
                builder.Append(ch);
            }
            return builder.ToString();
        }

        #endregion
    }
}
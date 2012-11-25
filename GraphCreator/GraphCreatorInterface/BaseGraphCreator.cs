using System;
using System.Collections.Generic;
using TextExtractor;

namespace GraphCreatorInterface
{
    public abstract class BaseGraphCreator : IGraphCreator
    {
        public abstract BaseGraph CreateGraphFromText(string text);

        public virtual BaseGraph CreateGraphFromFile(string filePath)
        {
            string text = new BaseTextExtractor().Extract(filePath);

            text = text.Replace(Environment.NewLine, " ");

            return CreateGraphFromText(text);
        }
    }
}

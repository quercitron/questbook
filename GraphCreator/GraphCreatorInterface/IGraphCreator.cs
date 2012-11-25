namespace GraphCreatorInterface
{
    public interface IGraphCreator
    {
        BaseGraph CreateGraphFromText(string text);

        BaseGraph CreateGraphFromFile(string filePath);
    }
}

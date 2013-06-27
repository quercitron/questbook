using BaseLib.Enumerables;
using BaseLib.Pathfinding.PathFormators;
using BaseLib.Pathfinding.PathStateGenerators;

namespace BaseLib.Pathfinding
{
    public class StandartPathfinderFactory : IPathfinderFactory
    {
        public IPathFinder Create(Book book, SearchAlgorithm algorithm)
        {
            IPathFinder pathFinder = null;
            
            switch (algorithm)
            {
                case SearchAlgorithm.Bfs:
                    pathFinder = new LongestPathFinder(new BfsPathStateGenerator());
                    break;
                case SearchAlgorithm.Dfs:
                    pathFinder = new LongestPathFinder(new DfsPathStateGenerator(false));
                    break;
                case SearchAlgorithm.RandomDfs:
                    pathFinder = new LongestPathFinder(new DfsPathStateGenerator(true));
                    break;
                case SearchAlgorithm.DiscoverNewParagraph:
                    pathFinder = new BasePathFinder(new BfsPathStateGenerator(), new DiscoverNewParagraphPathFormator(book.ParagraphsDictionary));
                    break;
            }

            return pathFinder;
        }
    }
}

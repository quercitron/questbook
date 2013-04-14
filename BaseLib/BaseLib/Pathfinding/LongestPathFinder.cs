using BaseLib.Pathfinding.PathFormators;
using BaseLib.Pathfinding.PathStateGenerators;

namespace BaseLib.Pathfinding
{
    public class LongestPathFinder : BasePathFinder
    {
        public LongestPathFinder(IPathStateGenerator pathStateGenerator)
            : base(pathStateGenerator, new LongestPathFormator())
        {
        }
    }
}

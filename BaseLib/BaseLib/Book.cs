using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using BaseLib.Enumerables;
using BaseLib.Pathfinding;
using BaseLib.Pathfinding.PathFormators;
using BaseLib.Pathfinding.PathStateGenerators;
using BlackWoodBook;
using CaptainSheltonBook;
using GraphCreatorInterface;

namespace BaseLib
{
    [Serializable]
    public class Book
    {
        private Book(string name, BaseGraph baseGraph)
        {
            Name = name;
            this.CreateDate = DateTime.Now;

            this.m_Paragraphs = new Dictionary<int, Paragraph>();
            this.m_Edges = new Dictionary<int, List<Edge>>();
            foreach (var vertex in baseGraph.Vertices)
            {
                var paragraph = new Paragraph(vertex);
                if (baseGraph.Descriptions.ContainsKey(vertex))
                {
                    paragraph.Description = baseGraph.Descriptions[vertex];
                }

                m_Paragraphs.Add(vertex, paragraph);
                m_Edges.Add(vertex, new List<Edge>());
            }

            foreach (var simpleEdge in baseGraph.Edges)
            {
                var edge = new Edge(GetParagraph(simpleEdge.From), GetParagraph(simpleEdge.To), true);
                m_Edges[simpleEdge.From].Add(edge);
            }

            AvailableItems = new List<ItemType>();
            m_IsSaved = false;
        }

        public Book(string filePath, IGraphCreator graphCreator, string name)
            : this(name, graphCreator.CreateGraphFromFile(filePath))
        {
            m_GraphCreator = graphCreator;
        }

        public Book(string filePath, IGraphCreator graphCreator)
            : this(filePath, graphCreator, Path.GetFileNameWithoutExtension(filePath))
        {
        }

        // TODO: move to fabric?
        public static Book CreateBook(string filePath, QuestBookType bookType)
        {
            IGraphCreator graphCreator = null;
            switch (bookType)
            {
                case QuestBookType.BlackWood:
                    graphCreator = new BlackWoodGraphCreator();
                    break;
                case QuestBookType.CaptainShelton:
                    graphCreator = new CaptainSheltonGraphCreator();
                    break;
            }
            return new Book(filePath, graphCreator);
        }

        public string Name { get; set; }

        public DateTime CreateDate { get; private set; }
        public DateTime LastUpdateDate { get; private set; }

        public List<ItemType> AvailableItems { get; private set; }

        private readonly Dictionary<int, Paragraph> m_Paragraphs;

        [OptionalField]
        private readonly IGraphCreator m_GraphCreator;

        [OptionalField]
        private bool m_IsSaved;
        public bool IsSaved
        {
            get
            {
                return m_IsSaved;
            }
            set
            {
                m_IsSaved = value;
            }
        }

        [OptionalField]
        private string m_SaveName;
        public string SaveName
        {
            get
            {
                return m_SaveName ?? Name + ".qbs";
            }
            set
            {
                m_SaveName = value;
            }
        }

        public Paragraph GetParagraph(int id)
        {
            return m_Paragraphs[id];
        }

        public List<Paragraph> Paragraphs
        {
            get { return m_Paragraphs.Values.ToList(); }
        }

        private readonly Dictionary<int, List<Edge>> m_Edges;

        public void AddNewItem(ItemType item)
        {
            AvailableItems.Add(item);
        }

        public List<SearchResultState> LastGeneratedWay { get; private set; }

        public SearchParameters LastSearchParameters { get; private set; }

        public List<Edge> GetEdges(int? from, int? to)
        {
            var result = new List<Edge>();

            if (!from.HasValue)
            {
                foreach (var edges in m_Edges.Values)
                {
                    result.AddRange(edges.Where(e => !to.HasValue || e.To.Id == to.Value));
                }
            }
            else
            {
                result.AddRange(m_Edges[from.Value].Where(e => !to.HasValue || e.To.Id == to.Value));
            }

            return result;
        }

        public void Save()
        {
            var path = SaveName;
            Save(path);
        }

        public void Save(string path)
        {
            var binaryFormatter = new BinaryFormatter();
            IsSaved = true;
            SaveName = Path.GetFileName(path);
            using (Stream stream = File.Open(path, FileMode.Create))
            {
                binaryFormatter.Serialize(stream, this);
            }
        }

        public static Book Load(string path)
        {
            var binaryFormatter = new BinaryFormatter();
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                return (Book) binaryFormatter.Deserialize(stream);
            }
        }

        public void RemoveItem(ItemType itemType)
        {
            foreach (var paragraph in Paragraphs)
            {
                paragraph.RecievedItems.RemoveAll(item => item.BasicItem == itemType);
            }

            foreach (var edges in m_Edges.Values)
            {
                foreach (var edge in edges)
                {
                    edge.RecievedItems.RemoveAll(item => item.BasicItem == itemType);
                    edge.RequestedItems.RemoveAll(item => item.BasicItem == itemType);
                }
            }

            AvailableItems.Remove(itemType);
        }

        public List<SearchResultState> GetWay(SearchParameters parameters)
        {
            LastSearchParameters = parameters;

            // remove unuse items
            parameters.StartState.Items.RemoveAll(item => !item.BasicItem.InUse || !item.BasicItem.IsProhibiting);

            var stopwatch = Stopwatch.StartNew();

            IPathFinder pathFinder = null;

            // TODO: Add mapping
            switch (parameters.Algorithm)
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
                    pathFinder = new BasePathFinder(new BfsPathStateGenerator(), new DiscoverNewParagraphPathFormator(m_Paragraphs));
                    break;
            }

            var result = pathFinder.FindPath(parameters.StartState, m_Edges);

            stopwatch.Stop();

            if (result != null)
            {
                HandleNewPath(result);
                LastGeneratedWay = result;
            }

            return result;
        }

        private void HandleNewPath(IList<SearchResultState> newPath)
        {
            MarkPreviousStates(newPath);

            foreach (var searchResultState in newPath)
            {
                var paragraph = GetParagraph(searchResultState.State.ParagraphNo);
                if (!paragraph.WasVisited)
                {
                    // TODO: Change mechanism
                    paragraph.WasVisited = true;
                    searchResultState.VisitedFirstTime = true;
                }
            }
        }

        private void MarkPreviousStates(IList<SearchResultState> newPath)
        {
            if (newPath == null || LastGeneratedWay == null)
            {
                return;
            }

            int[,] d = new int[newPath.Count + 1,LastGeneratedWay.Count + 1];
            bool[,] e = new bool[newPath.Count + 1,LastGeneratedWay.Count + 1];

            int i, j;

            for (i = 1; i <= newPath.Count; i++)
            {
                for (j = 1; j <= LastGeneratedWay.Count; j++)
                {
                    if (newPath[i - 1].State.ParagraphNo.Equals(LastGeneratedWay[j - 1].State.ParagraphNo))
                    {
                        d[i, j] = d[i - 1, j - 1] + 1;
                        e[i, j] = true;
                    }
                    else
                    {
                        if (d[i - 1, j] >= d[i, j - 1])
                        {
                            d[i, j] = d[i - 1, j];
                        }
                        else
                        {
                            d[i, j] = d[i, j - 1];
                        }
                    }
                }
            }

            i = newPath.Count;
            j = LastGeneratedWay.Count;

            while (i > 0 && j > 0)
            {
                if (e[i, j])
                {
                    newPath[i - 1].IsSame = true;
                    i--;
                    j--;
                }
                else
                {
                    if (d[i - 1, j] > d[i, j - 1])
                    {
                        i--;
                    }
                    else
                    {
                        j--;
                    }
                }
            }
        }

        public void AddNewEdge(int fromId, int toId)
        {
            // TODO: Add Check
            var newEdge = new Edge(GetParagraph(fromId), GetParagraph(toId), false);
            m_Edges[fromId].Add(newEdge);
        }

        public List<int> FindCycle(int p)
        {
            Dictionary<Paragraph, int> dict = new Dictionary<Paragraph, int>();
            Dictionary<Paragraph, Paragraph> way = new Dictionary<Paragraph, Paragraph>();

            return FindCycleDfs(GetParagraph(450), null, dict, way);
        }

        private List<int> FindCycleDfs(Paragraph p, Paragraph prev, Dictionary<Paragraph, int> dict, Dictionary<Paragraph, Paragraph> way)
        {
            way[p] = prev;
            dict[p] = 1;

            foreach (var edge in m_Edges[p.Id])
            {
                if (dict.ContainsKey(edge.To))
                {
                    if (dict[edge.To] == 1)
                    {
                        bool haveGold = false;

                        List<int> result = new List<int> {p.Id};
                        Paragraph current = p;

                        while (current != edge.To)
                        {
                            current = way[current];
                            result.Add(current.Id);
                        }

                        foreach (var id in result)
                        {
                            var paragraph = this.GetParagraph(id);
                            if (paragraph.RecievedItems.Count > 0)
                            {
                                if (paragraph.RecievedItems.Any(item => item.BasicItem.Name == "Gold"))
                                {
                                    haveGold = true;
                                }
                            }
                        }

                        if (haveGold)
                        {
                            result.Reverse();
                            return result;
                        }
                    }
                }
                else
                {
                    var res = FindCycleDfs(edge.To, p, dict, way);
                    if (res != null)
                    {
                        return res;
                    }
                }
            }

            dict[p] = 2;
            return null;
        }

        public void Update(BaseGraph baseGraph)
        {
            this.LastUpdateDate = DateTime.Now;

            foreach (var vertex in baseGraph.Vertices)
            {
                Paragraph paragraph;
                if (m_Paragraphs.ContainsKey(vertex))
                {
                    paragraph = m_Paragraphs[vertex];
                }
                else
                {
                    paragraph = new Paragraph(vertex);
                    m_Paragraphs.Add(vertex, paragraph);
                    m_Edges.Add(vertex, new List<Edge>());
                }

                if (baseGraph.Descriptions.ContainsKey(vertex))
                {
                    paragraph.Description = baseGraph.Descriptions[vertex];
                }                
            }

            foreach (var simpleEdge in baseGraph.Edges)
            {
                var edges = this.GetEdges(simpleEdge.From, simpleEdge.To);

                // TODO: Check condition when to add/not add new edge
                if (edges.Count == 0)
                {
                    var edge = new Edge(m_Paragraphs[simpleEdge.From], m_Paragraphs[simpleEdge.To], true);
                    m_Edges[simpleEdge.From].Add(edge);
                }
                // Mark edge as default
                if (edges.Count > 0 && !edges.Any(e => e.IsDefault))
                {
                    edges.First().IsDefault = true;
                }
            }
        }

        public void Update(string filePath)
        {
            Update(m_GraphCreator.CreateGraphFromFile(filePath));
        }

        public void ResetParagraphs()
        {
            foreach (var paragraph in this.Paragraphs)
            {
                paragraph.WasVisited = false;
            }
        }

        public void CleanPath()
        {
            foreach (var step in this.LastGeneratedWay)
            {
                if (step.VisitedFirstTime)
                {
                    this.GetParagraph(step.State.ParagraphNo).WasVisited = false;
                }
            }
            // TODO: change mechanics
            this.LastGeneratedWay = null;
        }
    }
}

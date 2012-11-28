using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using BaseLib.Comparers;
using BaseLib.Enumerables;
using BaseLib.Extensions;
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
        }

        public Book(string filePath, IGraphCreator graphCreator, string name)
            : this(name, graphCreator.CreateGraphFromFile(filePath))
        {
            m_GraphCreator = graphCreator;
        }

        public Book(string filePath, IGraphCreator graphCreator)
            : this(filePath, graphCreator, Path.GetFileName(filePath))
        {
        }

        public string Name { get; set; }

        public DateTime CreateDate { get; private set; }
        public DateTime LastUpdateDate { get; private set; }

        public List<ItemType> AvailableItems { get; private set; }

        private readonly Dictionary<int, Paragraph> m_Paragraphs;

        [OptionalField]
        private readonly IGraphCreator m_GraphCreator;

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

        // TODO: change to private

        private List<PersonState> GenerateNextStates(PersonState state)
        {
            var result = new List<PersonState>();

            foreach (var edge in this.m_Edges[state.ParagraphNo].Where(e => e.IsAcite).OrderByDescending(e => e.Priority, new PriorityComparer()))
            {
                if (ItemsWorker.EdgeIsAvailable(state, edge))
                {
                    var newState = ItemsWorker.MoveAlongTheEdge(state, edge);
                    if (newState != null)
                    {
                        result.Add(newState);
                    }
                    if (edge.Priority > 0)
                    {
                        break;
                    }
                }
            }

            return result;
        }

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

        public void Save(string path)
        {
            var binaryFormatter = new BinaryFormatter();
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

        private int m_StatesCount;

        public List<SearchResultState> GetWay(SearchParameters parameters)
        {
            LastSearchParameters = parameters;

            // remove unuse items
            parameters.StartState.Items.RemoveAll(item => !item.BasicItem.InUse);

            var stopwatch = Stopwatch.StartNew();

            m_StatesCount = 0;

            List<SearchResultState> result = null;
            switch (parameters.Algorithm)
            {
                case SearchAlgorithm.Bfs:
                    result = GetFurthestWay(parameters.StartState);
                    break;
                case SearchAlgorithm.Dfs:
                    result = GetDfsFurthestWay(parameters.StartState, false);
                    break;
                case SearchAlgorithm.RandomDfs:
                    result = GetDfsFurthestWay(parameters.StartState, true);
                    break;
            }

            stopwatch.Stop();

            MarkPreviousStates(result);
            LastGeneratedWay = result;

            return result;
        }

        private void MarkPreviousStates(IList<SearchResultState> newWay)
        {
            if (LastGeneratedWay == null)
            {
                return;
            }

            int[,] d = new int[newWay.Count + 1, LastGeneratedWay.Count + 1];
            bool[,] e = new bool[newWay.Count + 1, LastGeneratedWay.Count + 1];

            int i, j;

            for (i = 1; i <= newWay.Count; i++)
            {
                for (j = 1; j <= LastGeneratedWay.Count; j++)
                {
                    if (newWay[i - 1].State.ParagraphNo.Equals(LastGeneratedWay[j - 1].State.ParagraphNo))
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

            i = newWay.Count;
            j = LastGeneratedWay.Count;

            while (i > 0 && j > 0)
            {
                if (e[i, j])
                {
                    newWay[i - 1].IsSame = true;
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

        private List<SearchResultState> GetFurthestWay(PersonState startState)
        {
            var searchResult = RunSearch(startState);

            return FindLongestWay(searchResult);
        }

        private List<SearchResultState> GetDfsFurthestWay(PersonState startState, bool shuffle)
        {
            long time1, time2;

            var stopwatch = Stopwatch.StartNew();

            var searchResult = RunDfsSearch(startState, shuffle);

            stopwatch.Stop();
            time1 = stopwatch.ElapsedMilliseconds;
            stopwatch.Start();

            var searchResultStates = FindLongestWay(searchResult);

            stopwatch.Stop();
            time2 = stopwatch.ElapsedMilliseconds;

            return searchResultStates;
        }

        private Dictionary<PersonState, int> RunSearch(PersonState startState)
        {
            var stateDict = new Dictionary<PersonState, int>();
            stateDict.Add(startState, 0);

            var queue = new Queue<PersonState>();
            queue.Enqueue(startState);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var currentDist = stateDict[current];

                var nextStates = GenerateNextStates(current).Where(s => !stateDict.ContainsKey(s));
                foreach (var nextState in nextStates)
                {
                    stateDict.Add(nextState, currentDist + 1);
                    queue.Enqueue(nextState);
                }
            }

            return stateDict;
        }

        private Dictionary<PersonState, int> RunDfsSearch(PersonState startState, bool shuffle)
        {
            var stateDict = new Dictionary<PersonState, int>();

            Dfs(startState, 0, stateDict, shuffle);

            return stateDict;
        }

        private void Dfs(PersonState current, int distance, Dictionary<PersonState, int> stateDict, bool shuffle)
        {
            m_StatesCount++;
            if (m_StatesCount > 10000)
            {
                m_StatesCount -= 10000;
                GC.Collect();
            }

            stateDict.Add(current, distance);

            var nextStates = GenerateNextStates(current).ToList();

            if (shuffle)
            {
                nextStates.Shuffle();
            }

            foreach (var nextState in nextStates)
            {
                if (!stateDict.ContainsKey(nextState))
                {
                    Dfs(nextState, distance + 1, stateDict, shuffle);
                }
            }
        }

        private static List<SearchResultState> FindLongestWay(Dictionary<PersonState, int> searchResult)
        {
            var max = searchResult.Values.Max();
            var furthestState = searchResult.First(p => p.Value == max).Key;

            return WayToState(searchResult, furthestState);
        }

        private static List<SearchResultState> WayToState(Dictionary<PersonState, int> searchResult, PersonState furthestState)
        {
            var result = new List<SearchResultState>();
            while (furthestState != null)
            {
                result.Add(new SearchResultState(furthestState, searchResult[furthestState]));
                furthestState = furthestState.PreviousState;
            }

            result.Reverse();
            return result;
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
                if (!edges.Any(e => e.IsDefault))
                {
                    var edge = new Edge(m_Paragraphs[simpleEdge.From], m_Paragraphs[simpleEdge.To], true);
                    m_Edges[simpleEdge.From].Add(edge);   
                }                
            }
        }

        public void Update(string filePath)
        {
            Update(m_GraphCreator.CreateGraphFromFile(filePath));
        }
    }
}

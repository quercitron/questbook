using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        public Book(string name, BaseGraph baseGraph)
        {
            Name = name;
            CreateTime = DateTime.Now;

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
                var edge = new Edge(m_Paragraphs[simpleEdge.From], m_Paragraphs[simpleEdge.To], true);
                m_Edges[simpleEdge.From].Add(edge);
            }

            AvailableItems = new List<ItemType>();
        }

        public Book(string name, string filePath, IGraphCreator graphCreator)
            : this(name, graphCreator.CreateGraphFromFile(filePath))
        {
        }

        public Book(string filePath, IGraphCreator graphCreator)
            : this(Path.GetFileName(filePath), filePath, graphCreator)
        {
        }

        public string Name { get; set; }

        public DateTime CreateTime { get; private set; }

        public List<ItemType> AvailableItems { get; private set; }

        private readonly Dictionary<int, Paragraph> m_Paragraphs;
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

        public List<SearchResultState> GetWay(SearchParameters parameters)
        {
            // remove unuse items
            parameters.StartState.Items.RemoveAll(item => !item.BasicItem.InUse);

            var stopwatch = Stopwatch.StartNew();

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

            LastGeneratedWay = result;
            LastSearchParameters = parameters;

            return result;
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
            var stopwatch = Stopwatch.StartNew();
            var max = searchResult.Values.Max();
            var furthestState = searchResult.First(p => p.Value == max).Key;
            stopwatch.Stop();

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
            var newEdge = new Edge(m_Paragraphs[fromId], m_Paragraphs[toId], false);
            m_Edges[fromId].Add(newEdge);
        }

        public List<int> FindCycle(int p)
        {
            Dictionary<Paragraph, int> dict = new Dictionary<Paragraph, int>();
            Dictionary<Paragraph, Paragraph> way = new Dictionary<Paragraph, Paragraph>();

            return FindCycleDfs(m_Paragraphs[450], null, dict, way);
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
                            if (m_Paragraphs[id].RecievedItems.Count > 0)
                            {
                                if (m_Paragraphs[id].RecievedItems.Any(item => item.BasicItem.Name == "Gold"))
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
    }
}

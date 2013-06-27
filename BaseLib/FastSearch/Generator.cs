using System;
using System.Collections.Generic;
using System.Linq;

namespace FastSearch
{
    public class Generator
    {
        private static readonly PriorityComparer PriorityComparer = new PriorityComparer();

        public Dictionary<State, int> Generate(State startState)
        {
            var stateDict = new Dictionary<State, int>();
            stateDict.Add(startState, 0);

            var queue = new Queue<State>();
            queue.Enqueue(startState);

            _countables = new Dictionary<int, int>();
            _stateFactory = new StateFactory();

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

        private List<State> GenerateNextStates(State state)
        {
            var result = new List<State>();

            foreach (var edge in Edges[state.ParagraphNo].OrderByDescending(e => e.Priority, PriorityComparer))
            {
                if (EdgeIsAvailable(state, edge))
                {
                    var newState = MoveAlongTheEdge(state, edge);
                    if (newState != null)
                    {
                        result.Add(newState);
                        if (edge.Priority > 0)
                        {
                            break;
                        }
                    }
                }
            }

            return result;
        }

        private State MoveAlongTheEdge(State state, Edge edge)
        {
            var newState = _stateFactory.Create();
            newState.ParagraphNo = edge.End.Id;
            newState.Mask = state.Mask;
            newState.PrevState = state;

            foreach (var itemId in CountableIds)
            {
                var id = state.Id * 64 + itemId;
                if (_countables.ContainsKey(id))
                {
                    var newId = newState.Id * 64 + itemId;
                    _countables.Add(newId, _countables[id]);
                }
            }

            ApplyEvent(newState, edge);
            ApplyEvent(newState, edge.End);

            if ((newState.Mask | VitalMask) != newState.Mask)
            {
                return null;
            }

            return newState;
        }

        private void ApplyEvent(State state, Event e)
        {
            state.Mask &= e.RequestedItems.UncMask;
            foreach (var item in e.RequestedItems.CountableItems.Where(item => item.IsDisappearing))
            {
                var id = state.Id * 64 + item.Id;
                if (_countables.ContainsKey(id))
                {
                    _countables[id] -= item.Count;
                    if (_countables[id] <= 0)
                    {
                        _countables.Remove(id);
                        state.Mask ^= 1 << item.Id;
                    }
                }
            }

            state.Mask |= e.RecievedItems.Mask;
            foreach (var item in e.RecievedItems.CountableItems)
            {
                var id = state.Id * 64 + item.Id;
                if (_countables.ContainsKey(id))
                {
                    _countables[id] += item.Count;
                }
                else
                {
                    _countables.Add(id, item.Count);
                }
            }
        }

        private bool EdgeIsAvailable(State state, Edge edge)
        {
            // TODO: be carefull with InUse and IsProhibiting

            if ((state.Mask & edge.RequestedItems.Mask) != state.Mask)
            {
                return false;
            }

            foreach (var item in edge.RequestedItems.CountableItems)
            {
                var id = state.Id * 64 + item.Id;

                if (_countables[id] < item.Count || (_countables[id] == item.Count && ItemIsVital[item.Id]))
                {
                    return false;
                }
            }

            return true;
        }

        public Dictionary<int, List<Edge>> Edges { get; set; }

        public Int64 VitalMask { get; set; }

        public List<int> CountableIds { get; set; }

        public bool[] ItemIsVital { get; set; }

        private Dictionary<int, int> _countables;

        private StateFactory _stateFactory;
    }
}

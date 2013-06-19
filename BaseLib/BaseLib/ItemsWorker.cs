using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseLib
{
    public static class ItemsWorker
    {
        public static bool ItemsAreEqual(IList<ItemUnit> a, IList<ItemUnit> b)
        {
            if (a.Count != b.Count)
            {
                return false;
            }

            return a.Select(b.IndexOf).All(index => index >= 0);
        }

        private static void RemoveUnnecessaryItems(IList<ItemUnit> resultItems)
        {
            int i = 0;
            while (i < resultItems.Count)
            {
                if (resultItems[i].Count <= 0)
                {
                    resultItems[i] = resultItems[resultItems.Count - 1];
                    resultItems.RemoveAt(resultItems.Count - 1);
                }
                else
                {
                    if (resultItems[i].BasicItem.IsUnique)
                    {
                        resultItems[i].Count = 1;
                    }

                    // TODO: Remove limitation
                    if (resultItems[i].Count > 50)
                    {
                        resultItems[i].Count = 50;
                    }

                    i++;
                }
            }
        }

        public static bool EdgeIsAvailable(PersonState state, Edge edge)
        {
            // TODO: be carefull with InUse and IsProhibiting
            if (edge.RequestedItems.Any(item => !item.BasicItem.InUse && item.BasicItem.IsProhibiting))
            {
                return false;
            }

            foreach (var item in edge.RequestedItems.Where(item => item.BasicItem.IsProhibiting))
            {
                var current = state.Items.Find(a => a.BasicItem == item.BasicItem);
                if (current == null || current.Count < item.Count)
                {
                    return false;
                }
            }

            return true;
        }

        public static PersonState MoveAlongTheEdge(PersonState state, Edge edge)
        {
            var resultItems = state.Items.Select(item => new ItemUnit(item)).ToList();
            foreach (var item in edge.RequestedItems.Where(a => a.IsDisappearing && a.BasicItem.InUse))
            {
                resultItems.Find(a => a.BasicItem == item.BasicItem).Count -= item.Count;
            }
            // TODO: be careful with IsProhibiting
            foreach (var item in edge.To.RequestedItems.Where(a => a.BasicItem.IsProhibiting))
            {
                var itemUnit = resultItems.Find(a => a.BasicItem == item.BasicItem);
                if (itemUnit != null)
                {
                    if (item.Count >= 0)
                    {
                        itemUnit.Count -= item.Count;
                    }
                    else
                    {
                        itemUnit.Count = 0;
                    }
                }
                if (item.BasicItem.IsVital && (itemUnit == null || itemUnit.Count <= 0))
                {
                    return null;
                }
            }

            AddItems(resultItems, edge.RecievedItems);
            AddItems(resultItems, edge.To.RecievedItems);

            RemoveUnnecessaryItems(resultItems);

            return new PersonState(edge.To.Id, resultItems, state, edge);
        }

        private static void AddItems(List<ItemUnit> resultItems, IEnumerable<ItemUnit> recievedItems)
        {
            foreach (var item in recievedItems.Where(item => item.BasicItem.InUse))
            {
                var current = resultItems.Find(x => x.BasicItem == item.BasicItem);
                if (current == null)
                {
                    resultItems.Add(new ItemUnit(item));
                }
                else
                {
                    current.Count += item.Count;
                }
            }
        }
    }
}

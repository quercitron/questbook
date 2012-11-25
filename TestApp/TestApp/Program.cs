using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using BaseLib;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            List<SimpleEdge> edges = new List<SimpleEdge>();
            /*edges.Add(new SimpleEdge(0, 1));
            edges.Add(new SimpleEdge(1, 2));
            edges.Add(new SimpleEdge(2, 0));
            edges.Add(new SimpleEdge(1, 3));

            Book book = new Book("book1", 4, edges);

            ItemType it = new ItemType("item1", true);
            book.AddNewItem(it);

            var x = book.GetEdges(1, 3).First();
            x.AddRequestedItem(new RequestedItemUnit(it));

            book.GetParagraph(2).AddRecievedItem(new ItemUnit(it));*/

            edges.Add(new SimpleEdge(0, 1));
            edges.Add(new SimpleEdge(0, 2));
            edges.Add(new SimpleEdge(1, 3));
            edges.Add(new SimpleEdge(2, 3));
            edges.Add(new SimpleEdge(2, 4));
            edges.Add(new SimpleEdge(4, 3));
            edges.Add(new SimpleEdge(3, 5));
            edges.Add(new SimpleEdge(5, 3));
            edges.Add(new SimpleEdge(3, 6));

            Book book = new Book("book1", 7, edges);

            ItemType gold = new ItemType("gold", false);
            ItemType am = new ItemType("am", true);

            book.GetEdges(0, 1).First().AddRequestedItem(new RequestedItemUnit(gold, 2));
            book.GetEdges(3, 5).First().AddRequestedItem(new RequestedItemUnit(gold, 1));
            book.GetEdges(3, 6).First().AddRequestedItem(new RequestedItemUnit(am, 4));

            book.GetParagraph(1).AddRecievedItem(new ItemUnit(am, 1));
            book.GetParagraph(4).AddRecievedItem(new ItemUnit(am, 1));
            book.GetParagraph(5).AddRecievedItem(new ItemUnit(am, 1));

            var searchResults = book.RunSearch(new PersonState(0, new List<ItemUnit> { new ItemUnit(gold, 3) }));

            using (TextWriter writer = File.CreateText("output.txt"))
            {
                foreach (var result in searchResults)
                {
                    writer.WriteLine("{0} Distance: {1}", result.Key, result.Value);
                }
            }
        }
    }
}

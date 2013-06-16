using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using BaseLib;
using BaseLib.Enumerables;
using BlackWoodBook;
using CaptainSheltonBook;
using GraphCreatorInterface;
using PDFBoxParser;
using PdfParserInterfaces;

namespace TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var writer = File.CreateText("output.txt");
            try
            {
                //var creator = new CaptainSheltonGraphCreator();
                //creator.CreateGraphFromFile("Braslavskiy_Tayna_kapitana_Sheltona.240266.fb2");
                XElement book = XElement.Load("Braslavskiy_Tayna_kapitana_Sheltona.240266.fb2");
                foreach (var element in book.Elements("body").Elements("section").Where(s => s.HasAttributes))
                {
                    writer.WriteLine(element);
                    writer.WriteLine(element.Name);
                    writer.WriteLine("# {0}", element.Element("title").Element("p").Value);
                    var refs = element.Elements("p").Elements("a");
                    foreach (var @ref in refs)
                    {
                        writer.WriteLine(@ref.Value.Trim('(', ')'));
                    }
                    var text = string.Join(Environment.NewLine, element.Elements("p").Select(p => p.Value));
                    writer.WriteLine("({0})", text);
                }
                //writer.WriteLine(book.Element("description"));
            }
            catch (Exception e)
            {
                writer.WriteLine(e);
            }
            writer.Dispose();
        }

        private static void FindNecklace()
        {
/*var book = new Book("blackwood.txt", new BlackWoodGraphCreator(), "Forest Of Doom");
                book.Save("save2.txt");*/

            var book = Book.Load("save.txt");

            var edges = book.GetEdges(null, null);
            string itemName = "Necklace";
            foreach (var edge in edges)
            {
                if (edge.RequestedItems.Any(item => item.BasicItem.Name.Contains(itemName)))
                {
                    Console.WriteLine(edge.From.Id);
                }
                if (edge.RecievedItems.Any(item => item.BasicItem.Name.Contains(itemName)))
                {
                    Console.WriteLine(edge.From.Id);
                }
            }

            foreach (var paragraph in book.Paragraphs)
            {
                if (paragraph.RequestedItems.Any(item => item.BasicItem.Name.Contains(itemName)))
                {
                    Console.WriteLine(paragraph.Id);
                }
                if (paragraph.RecievedItems.Any(item => item.BasicItem.Name.Contains(itemName)))
                {
                    Console.WriteLine(paragraph.Id);
                }
            }

            /*foreach (var item in book.AvailableItems)
                {
                    item.IsProhibiting = true;
                }*/

            //book.m_GraphCreator = new BlackWoodGraphCreator();
            /*book.Update("Майкл Фрост - Чернолесье.doc");*/
            /*book.Save("save2.txt");*/
        }

        private static void SearchCycles()
        {
            Book book;

            //var book = new Book("Forest Of Doom", "blackwood.txt", new BlackWoodGraphCreator());

            //book.Save("save.txt");

            book = Book.Load("save.txt");

            var dict = new Dictionary<int, bool>();

            List<int> cycle = null;

            Thread thread = new Thread(() => cycle = book.FindCycle(1), 1024 * 1024 * 1024);

            thread.Start();
            thread.Join();
            using (TextWriter writer = File.CreateText("output.txt"))
            {
                foreach (var p in cycle)
                {
                    writer.WriteLine(p);
                }
            }
        }

        private static void PrintWay(Book book)
        {
            var searchResults = book.GetWay(new SearchParameters(new PersonState(1), SearchAlgorithm.Bfs));

            using (TextWriter writer = File.CreateText("output.txt"))
            {
                foreach (var result in searchResults)
                {
                    writer.WriteLine("{0} Distance: {1}", result.State, result.Distance);
                }
            }
        }

        private static void PrintGraph(BaseGraph graph)
        {
            using (TextWriter writer = File.CreateText("output.txt"))
            {
                writer.Write(graph);
            }
        }

        private static void TestPdfParser()
        {
            IPdfParser parser;
            try
            {
                parser = new PdfBoxParser();
            }
            catch (Exception e)
            {
                Console.WriteLine("1: {0}", e);
                return;
            }

            string result;
            try
            {
                result = parser.ParseFile("Tri_Dorogi.book.PDF");
                result = result.Replace("-" + Environment.NewLine, string.Empty);
            }
            catch (Exception e)
            {
                Console.WriteLine("2: {0}", e);
                return;
            }

            using (var writer = File.CreateText("output.txt"))
            {
                writer.Write(result);
            }
        }
    }
}

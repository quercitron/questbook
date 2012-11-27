using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using BaseLib;
using BaseLib.Enumerables;
using BlackWoodBook;
using GraphCreatorInterface;
using PDFBoxParser;
using PdfParserInterfaces;

namespace TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                /*var book = new Book("blackwood.txt", new BlackWoodGraphCreator(), "Forest Of Doom");
                book.Save("save.txt");*/

                var book = Book.Load("save.txt");
                book.m_GraphCreator = new BlackWoodGraphCreator();
                /*book.Update("Майкл Фрост - Чернолесье.doc");*/
                book.Save("save2.txt");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
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

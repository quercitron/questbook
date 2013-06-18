using System;
using System.IO;
using System.Linq;
using PDFBoxParser;
using TextExtractorInterface;

namespace TextExtractor
{
    public class BaseTextExtractor : ITextExtractor
    {
        private readonly string[] PlainTextFormats = new[] {".txt", ".fb2"};

        public string Extract(string filePath)
        {
            var extension = Path.GetExtension(filePath);

            if (extension == ".pdf")
            {
                var pdfParser = new PdfBoxParser();

                return pdfParser.ParseFile(filePath);
            }

            if (PlainTextFormats.Contains(extension))
            {
                using (TextReader reader = File.OpenText(filePath))
                {
                    return reader.ReadToEnd();
                }
            }

            throw new ArgumentException(string.Format("Extension '{0}' is not supported", extension));
        }
    }
}

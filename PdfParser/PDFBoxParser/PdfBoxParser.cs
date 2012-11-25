using PdfParserInterfaces;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;

namespace PDFBoxParser
{
    public class PdfBoxParser : IPdfParser
    {
        public string ParseFile(string path)
        {
            PDDocument doc = PDDocument.load(path);
            PDFTextStripper stripper = new PDFTextStripper();
            string text = stripper.getText(doc);
            doc.close();
            return text;
        }
    }
}

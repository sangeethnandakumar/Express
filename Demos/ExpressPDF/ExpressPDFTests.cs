using Express.Interop.Constructs;
using Express.PDF;
using Express.PDF.PDFObjects;

namespace Demos.ExpressPDF
{
    public static class ExpressPDFTests
    {
        public static void SplitPDFPages()
        {
            //Insance
            IPDFOperations op = new PDFOperations();

            //Open existing PDF
            var document = op.OpenPDF(@"D:\ExpressProject\Express\Demos\bin\Debug\net6.0\ExpressPDF\sample.pdf");
            //Focus interested pages
            var interestedPages = new PageRange("pages 1");
            //Extract them
            var pages = op.ExtractPages(document, interestedPages);
            //Make new PDF with extracted pages
            var newDocument = op.MakePDFFile(pages);
            //Save as new PDF
            op.SavePDF(newDocument, @"D:\ExpressProject\Express\Demos\bin\Debug\net6.0\ExpressPDF\splitted.pdf");
        }

        public static void MergePDFPages()
        {
            //Insance
            IPDFOperations op = new PDFOperations();

            //Open existing PDF
            var document = op.OpenPDF(@"D:\ExpressProject\Express\Demos\bin\Debug\net6.0\ExpressPDF\sample.pdf");
            //Focus interested pages
            var setA = new PageRange("pages 1-6");
            var setB = new PageRange("pages 10-12, 20-22");
            var setC = new PageRange("pages 50-55,66,88,99");
            //Extract pages
            var pagesA = op.ExtractPages(document, setA);
            var pagesB = op.ExtractPages(document, setB);
            var pagesC = op.ExtractPages(document, setC);
            //Merge pages
            var mergedPages = op.MergePages(pagesA, pagesB, pagesC);
            //Make new PDF with extracted pages
            var newDocument = op.MakePDFFile(mergedPages);
            //Save as new PDF
            op.SavePDF(newDocument, @"D:\ExpressProject\Express\Demos\bin\Debug\net6.0\ExpressPDF\merged.pdf");
        }

        public static void RemovePDFPages()
        {
            //Insance
            IPDFOperations op = new PDFOperations();

            //Open existing PDF
            var document = op.OpenPDF(@"D:\ExpressProject\Express\Demos\bin\Debug\net6.0\ExpressPDF\sample.pdf");
            var allPages = op.GetAllPages(document);
            //Focus interested pages
            var pagesToDelete = new PageRange("pages 100-228");
            //Delete them
            var afterRemoval = op.RemovePages(allPages, pagesToDelete);
            //Make new PDF with new page collections
            var newDocument = op.MakePDFFile(afterRemoval);
            //Save as new PDF
            op.SavePDF(newDocument, @"D:\ExpressProject\Express\Demos\bin\Debug\net6.0\ExpressPDF\removed.pdf");
        }

        public static void AnalyzePDF()
        {
            //Insance
            IPDFOperations op = new PDFOperations();

            //Open existing PDF
            var document = op.OpenPDF(@"D:\ExpressProject\Express\Demos\bin\Debug\net6.0\ExpressPDF\sample.pdf");

            //Discover words from page 1
            List<PDFWord> words = op.DiscoverAllWords(document, 1);            
            //Approximate lines
            List<PDFLine> lines = op.ApproximateAllLines(words);
            //Approximate text blocks
            List<PDFTextBlock> blocks = op.ApproximateAllTextBlocks(lines);

            //Output
            foreach (var section in blocks)
            {
                Console.WriteLine(section.ToString());
            }
        }

        public static void MergePDFFiles()
        {
            //Insance
            IPDFOperations op = new PDFOperations();

            //Open existing PDF
            var documentA = op.OpenPDF(@"D:\ExpressProject\Express\Demos\bin\Debug\net6.0\ExpressPDF\sample.pdf");
            var documentB = op.OpenPDF(@"D:\ExpressProject\Express\Demos\bin\Debug\net6.0\ExpressPDF\merged.pdf");
            var documentC = op.OpenPDF(@"D:\ExpressProject\Express\Demos\bin\Debug\net6.0\ExpressPDF\removed.pdf");
            //Focus interested pages from multiple PDF's
            //PDF-A
            var pagesA = op.ExtractPages(documentA, new PageRange("pages 1-6"));
            var pagesB = op.GetAllPages(documentB);
            var pagesC = op.ExtractPages(documentC, new PageRange(1,3,5,6));
            //Merge pages
            var mergedPages = op.MergePages(pagesA, pagesB, pagesC);
            //Make new PDF with extracted pages
            var newDocument = op.MakePDFFile(mergedPages);
            //Save as new PDF
            op.SavePDF(newDocument, @"D:\ExpressProject\Express\Demos\bin\Debug\net6.0\ExpressPDF\merged.pdf");
        }

        public static void SplitPDFByFileSize()
        {
            //Insance
            IPDFOperations op = new PDFOperations();

            //Open existing PDF
            var document = op.OpenPDF(@"D:\ExpressProject\Express\Demos\bin\Debug\net6.0\ExpressPDF\sample.pdf");
            //Get multiple PDF files based by splitting with filesize
            var splittedDocuments = op.SplitPDFByFileSize(document, @"D:\ExpressProject\Express\Demos\bin\Debug\net6.0\ExpressPDF", 1024 * 2);
            //Save each of them
            foreach (var doc in splittedDocuments)
            {
                op.SavePDF(doc, $@"D:\ExpressProject\Express\Demos\bin\Debug\net6.0\ExpressPDF\chunk-{Guid.NewGuid()}.pdf");
            }
        }
    }
}

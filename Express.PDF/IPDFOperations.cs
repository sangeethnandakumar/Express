using Express.Interop.Constructs;
using Express.PDF.Constructs;
using Express.PDF.PDFObjects;
using System.Collections.Generic;

namespace Express.PDF
{
    public interface IPDFOperations
    {
        //Basic Operations
        PDFFile OpenPDF(string sourceLocation);
        PDFPageCollection GetAllPages(PDFFile pdfFile);
        PDFPageCollection ExtractPages(PDFFile pdfFile, PageRange pageRange);
        PDFPageCollection MergePages(params PDFPageCollection[] pageCollection);
        PDFPageCollection RemovePages(PDFPageCollection pageCollection, PageRange pageRange);
        PDFFile MakePDFFile(PDFPageCollection pageCollection);
        void SavePDF(PDFFile pdfFile, string destinationLocation);

        //Advanced Features
        public List<PDFFile> SplitPDFByFileSize(PDFFile pdfFile, string outputLoc, long limit);
        bool LockPDF(string sourceLocation, string destLocation, string password);
        bool UnlockPDF(string sourceLocation, string destLocation, string password);

        //Aproximate Discovery features
        List<PDFWord> DiscoverAllWords(PDFFile pdfFile, int pageNumber);
        List<PDFLine> ApproximateAllLines(List<PDFWord> words, double approximationThreashold = 2);
        List<PDFTextBlock> ApproximateAllTextBlocks(List<PDFLine> lines, double approximationThreashold = 15);
    }
}

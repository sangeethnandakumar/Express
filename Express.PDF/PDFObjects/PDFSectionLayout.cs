using Express.PDF.Constructs;
using System.Collections.Generic;

namespace Express.PDF.PDFObjects
{
    public class PDFSectionLayout : PDFObject
    {
        public string Name { get; set; }
        public List<PDFTextBlock> TextBlocks { get; set; }
        public PDFSelection Bounds { get; set; }
        public PDFCentroid Centroid { get; set; }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}

using Express.PDF.Constructs;
using System.Collections.Generic;

namespace Express.PDF.PDFObjects
{
    public class PDFTextBlock : PDFObject
    {
        public string Name { get; set; }
        public List<PDFLine> Lines { get; set; }
        public PDFSelection Bounds { get; set; }
        public PDFCentroid Centroid { get; set; }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}

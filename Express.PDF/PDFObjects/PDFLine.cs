using Express.PDF.Constructs;
using System.Collections.Generic;

namespace Express.PDF.PDFObjects
{
    public class PDFLine : PDFObject
    {
        public string Name { get; set; }
        public List<PDFWord> Words { get; set; }
        public PDFSelection Bounds { get; set; }
        public PDFCentroid Centroid { get; set; }

        public override string ToString()
        {
            return $"Line: {Name}";
        }
    }
}

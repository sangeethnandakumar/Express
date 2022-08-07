using Express.PDF.Constructs;
using System;
using System.Collections.Generic;

namespace Express.PDF.PDFObjects
{
    public class PDFWord : PDFObject
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public List<PDFLetter> Letters { get; set; }
        public PDFSelection Bounds { get; set; }
        public PDFCentroid Centroid { get; set; }

        public override string ToString()
        {
            return $"Word: {Name}";
        }
    }
}

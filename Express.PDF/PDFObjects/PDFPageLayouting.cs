using Express.PDF.Constructs;
using System.Collections.Generic;

namespace Express.PDF.PDFObjects
{
    public class PDFPageLayouting : PDFObject
    {
        public List<PDFSectionLayout> SectionLayouts { get; set; }

    }
}

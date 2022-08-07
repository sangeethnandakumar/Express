using System.Collections.Generic;
using System.Linq;

namespace Express.PDF.Constructs
{
    public class PDFPageCollection
    {
        public List<PDFPage> Pages { get; set; }
        public PDFFile PDFFile { get; set; }
        internal PDFFile CollectionsPDFFile { get; set; }
        public override string ToString()
        {
            return $"{Pages.FirstOrDefault()} ⇾ {Pages.LastOrDefault()}";
        }
    }
}

using Express.PDF.Constructs;
using System.Collections.Generic;

namespace Express.PDF.PDFObjects
{
    public class PDFText : PDFObject
    {
        public string Name { get; set; }
        public List<string> Text { get; set; }

        public override string ToString()
        {
            return $"{Name} = [{string.Join(',', Text)}]";
        }
    }
}

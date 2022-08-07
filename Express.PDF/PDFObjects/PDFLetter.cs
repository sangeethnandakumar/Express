using Express.PDF.Constructs;

namespace Express.PDF.PDFObjects
{
    public class PDFLetter : PDFObject
    {
        public string Name { get; set; }
        public PDFSelection Bounds { get; set; }

        public override string ToString()
        {
            return $"Letter: {Name}";
        }
    }
}

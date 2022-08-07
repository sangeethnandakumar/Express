namespace Express.PDF.Constructs
{
    public class PDFPage
    {
        public int PageNumber { get; set; }

        public override string ToString()
        {
            return "Page: " + PageNumber;
        }
    }
}

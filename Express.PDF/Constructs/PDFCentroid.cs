namespace Express.PDF.Constructs
{
    public class PDFCentroid
    {
        public double X { get; set; }
        public double Y { get; set; }

        public override string ToString()
        {
            return $"[X: {X}| Y: {Y}]";
        }
    }
}

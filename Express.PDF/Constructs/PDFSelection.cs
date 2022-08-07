namespace Express.PDF.Constructs
{
    public class PDFSelection
    {
        public double Top { get; set; }
        public double Left { get; set; }
        public double Bottom { get; set; }
        public double Right { get; set; }

        public override string ToString()
        {
            return $"[Top: {Top}| Left: {Left}| Bottom: {Bottom}| Right: {Right}]";
        }
    }
}

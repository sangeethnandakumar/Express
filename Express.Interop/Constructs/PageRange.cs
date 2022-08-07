using System.Collections.Generic;
using System.Linq;

namespace Express.Interop.Constructs
{
    public enum RangeMode
    {
        RANGESTRING,
        EXPLICIT,
        PAGELIST
    }

    public class PageRange
    {
        public RangeMode RangeMode { get; set; }
        public string RangeString { get; set; }
        public List<int> PageNumbers { get; set; } = new List<int>();

        //Pages 1-100/Page 112/Pages 134-146, 168-119
        public PageRange(string rangeString)
        {
            var pageRange = RangeOperations.GetPageRange(rangeString);
            RangeString = pageRange.RangeString;
            PageNumbers = pageRange.PageNumbers;
            RangeMode = RangeMode.RANGESTRING;
        }

        internal PageRange(string rangeString, List<int> range)
        {
            RangeString = rangeString;
            PageNumbers = range;
            RangeMode = RangeMode.RANGESTRING;
        }

        //1,2,3,4...
        public PageRange(params int[] range)
        {
            RangeString = $"{range.FirstOrDefault()} - {range.LastOrDefault()}";
            PageNumbers.AddRange(range);
            RangeMode = RangeMode.EXPLICIT;
        }

        //[1,2,3,4]
        public PageRange(List<int> range)
        {
            RangeString = $"{range.FirstOrDefault()} - {range.LastOrDefault()}";
            PageNumbers.AddRange(range);
            RangeMode = RangeMode.PAGELIST;
        }

        public override string ToString()
        {
            return "Pages: " + string.Join(", ", PageNumbers);
        }
    }
}

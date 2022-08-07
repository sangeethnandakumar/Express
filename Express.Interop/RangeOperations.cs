using Express.Interop.Constructs;
using Express.Interop.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Express.Interop
{
    internal static class RangeOperations
    {
        public static PageRange GetPageRange(string rangeString)
        {
            var directiveIndex = rangeString.ToLower().IndexOf("page");
            if (directiveIndex != -1)
            {
                string[] data;
                data = rangeString.ToLower().Split(new[] { "page" }, StringSplitOptions.None);
                if (data.Length == 0)
                {
                    data = rangeString.Split(new[] { "page" }, StringSplitOptions.None);
                }
                if (data.Length != 0)
                {
                    var pages = GetPages(rangeString.Substring(directiveIndex));
                    return new PageRange(rangeString, pages);
                }
            }
            throw new InvalidPageRangeException($"Unable to determine page ranges from range string: '{rangeString}'");
        }

        private static List<int> GetPages(string pageDirective)
        {
            //Pages 26-29
            //Pages 26-29, 12-3, 12-14
            var type1 = Regex.IsMatch(pageDirective.ToLower().Trim(), @"^pages \d+(?:-\d+)?(?:,\s*\d+(?:-\d+)?)*$");
            if (type1)
            {
                return Type1Parser(pageDirective);
            }
            //Page 298
            var type2 = Regex.IsMatch(pageDirective.ToLower().Trim(), @"^page \d+(?:-\d+)?(?:,\s*\d+(?:-\d+)?)*$");
            if (type2)
            {
                return Type2Parser(pageDirective);
            }
            //Pages 298/Page 12
            var type3 = pageDirective.Contains("/");
            if (type3)
            {
                return Type3Parser(pageDirective);
            }
            return null;
        }

        private static List<int> Type1Parser(string type)
        {
            var pageNumbers = new List<int>();
            type = type.ToLower().Replace("pages", string.Empty);
            foreach (var range in type.Split(','))
            {
                if (range.Trim().Split('-').Count() == 2)
                {
                    var startRange = int.Parse(range.Trim().Split('-')[0]);
                    var endRange = int.Parse(range.Trim().Split('-')[1]);
                    var pages = GenerateRanges(startRange, endRange);
                    pageNumbers.AddRange(pages);
                    continue;
                }
                pageNumbers.AddRange(Type2Parser(range));
            }
            return pageNumbers;
        }

        private static List<int> Type2Parser(string type)
        {
            var pageNumbers = new List<int>();
            type = type.ToLower().Replace("page", string.Empty).Trim();
            pageNumbers.Add(int.Parse(type));
            return pageNumbers;
        }

        private static List<int> Type3Parser(string type)
        {
            var pageNumbers = new List<int>();
            foreach (var part in type.Split('/'))
            {
                if (part.ToLower().StartsWith("pages"))
                {
                    pageNumbers.AddRange(Type1Parser(part));
                }
                else if (part.ToLower().StartsWith("page"))
                {
                    pageNumbers.AddRange(Type2Parser(part));
                }
            }
            return pageNumbers;
        }

        private static IEnumerable<int> GenerateRanges(int begin, int end)
        {
            int cur = begin;
            while (cur <= end)
            {
                yield return cur;
                cur++;
            }
        }
    }

}

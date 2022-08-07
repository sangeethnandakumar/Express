using Humanizer;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Express.PDF.Constructs
{
    public class PDFFile
    {
        internal long Size { get; set; }

        //Public
        public Guid Id { get; } = Guid.NewGuid();
        public string FileSize { get; set; }
        public string FileName { get; set; }
        public string DirectoryName { get; set; }
        public int TotalPages { get; set; }
        public List<PDFPage> Pages { get; set; }

        //Internal
        internal PdfDocument Document { get; set; }
        internal MemoryStream Stream { get; set; }

        public PDFFile()
        {
            Pages = new List<PDFPage>();
            Size = Stream == null? 0 : Stream.Length;
            FileSize = Size.Bytes().Humanize();
        }

        public override string ToString()
        {
            return $"{Size.Bytes().Humanize()} | {Pages.FirstOrDefault()} ⇾ {Pages.LastOrDefault()}";
        }
    }
}

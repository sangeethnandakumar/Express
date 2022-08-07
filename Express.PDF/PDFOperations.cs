using Express.Interop.Constructs;
using Express.PDF.Constructs;
using Express.PDF.PDFObjects;
using Humanizer;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UglyToad.PdfPig.Content;

namespace Express.PDF
{
    public class PDFOperations : IPDFOperations
    {
        #region PUBLIC FUNCTIONS
        /// <summary>
        /// This function is used to load a PDF to memory
        /// </summary>
        /// <param name="sourceLocation"></param>
        /// <returns></returns>
        public PDFFile OpenPDF(string sourceLocation)
        {
            //Open PDF
            PdfDocument PDFDoc = PdfReader.Open(sourceLocation, PdfDocumentOpenMode.Modify);
            //Generate Page Info
            var pages = new List<PDFPage>();
            for (var i = 1; i <= PDFDoc.Pages.Count; i++)
            {
                var page = new PDFPage
                {
                    PageNumber = i
                };
                pages.Add(page);
            }
            //Return
            var pdfFile = new PDFFile
            {
                DirectoryName = Path.GetDirectoryName(sourceLocation),
                FileName = Path.GetFileName(sourceLocation),
                TotalPages = pages.Count,
                Document = PDFDoc,
                Pages = pages,
                Stream = new MemoryStream()
            };
            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            PDFDoc.Save(pdfFile.Stream);
            return pdfFile;
        }

        /// <summary>
        /// This function is used to extract interested pages from a PDFFile
        /// </summary>
        /// <param name="pdfFile"></param>
        /// <param name="pageRange"></param>
        /// <returns></returns>
        public PDFPageCollection ExtractPages(PDFFile pdfFile, PageRange pageRange)
        {
            PdfDocument PDFDoc = PdfReader.Open(pdfFile.Stream, PdfDocumentOpenMode.Import);
            PdfDocument PDFNewDoc = new PdfDocument();
            for (int i = 0; i < PDFDoc.Pages.Count; i++)
            {
                if (pageRange.PageNumbers.Contains(i + 1))
                {
                    PDFNewDoc.AddPage(PDFDoc.Pages[i]);
                }
            }
            var newPDFFile = RebuildPDFFile(PDFNewDoc, pdfFile);
            return new PDFPageCollection
            {
                PDFFile = pdfFile,
                CollectionsPDFFile = newPDFFile,
                Pages = newPDFFile.Pages
            };
        }

        /// <summary>
        /// Makes a PDF file from set of pages
        /// </summary>
        /// <param name="pageCollection"></param>
        /// <returns></returns>
        public PDFFile MakePDFFile(PDFPageCollection pageCollection)
        {
            var pdfFile = pageCollection.CollectionsPDFFile;
            return pdfFile;
        }

        /// <summary>
        /// Function used to merge PDF Page collection into one
        /// </summary>
        /// <param name="pageCollection"></param>
        /// <returns></returns>
        public PDFPageCollection MergePages(params PDFPageCollection[] pageCollection)
        {
            PdfDocument PDFNewDoc = new PdfDocument();
            foreach (var col in pageCollection)
            {
                var pdfDocument = PdfReader.Open(col.CollectionsPDFFile.Stream, PdfDocumentOpenMode.Import);
                for (int i = 0; i < pdfDocument.Pages.Count; i++)
                {
                    PDFNewDoc.AddPage(pdfDocument.Pages[i]);
                }
            }
            var newPDFFile = RebuildPDFFile(PDFNewDoc, pageCollection.FirstOrDefault().PDFFile);
            return new PDFPageCollection
            {
                CollectionsPDFFile = newPDFFile,
                PDFFile = pageCollection.FirstOrDefault().PDFFile,
                Pages = newPDFFile.Pages
            };
        }

        /// <summary>
        /// Function to get the page collection of a PDF File
        /// </summary>
        /// <param name="pdfFile"></param>
        /// <returns></returns>
        public PDFPageCollection GetAllPages(PDFFile pdfFile)
        {
            var pages = new List<PDFPage>();
            for (int i = 0; i < pdfFile.Pages.Count; i++)
            {
                pages.Add(new PDFPage
                {
                    PageNumber = i + 1
                });
            }
            return new PDFPageCollection
            {
                PDFFile = pdfFile,
                CollectionsPDFFile = pdfFile,
                Pages = pages
            };
        }

        /// <summary>
        /// Function to discover all words from a specified page
        /// </summary>
        /// <param name="pdfFile"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public List<PDFWord> DiscoverAllWords(PDFFile pdfFile, int pageNumber)
        {

            var wordList = new List<PDFWord>();
            using (var document = UglyToad.PdfPig.PdfDocument.Open(pdfFile.Stream))
            {
                var page = document.GetPage(pageNumber);

                IReadOnlyList<Letter> letters = page.Letters;
                IEnumerable<Word> words = page.GetWords();
                foreach (var word in words)
                {
                    wordList.Add(new PDFWord
                    {
                        Name = word.Text,
                        Letters = word.Letters.Select(l => new PDFLetter
                        {
                            Name = l.Value,
                            Bounds = new PDFSelection
                            {
                                Top = l.GlyphRectangle.Left,
                                Left = l.GlyphRectangle.Top,
                                Bottom = l.GlyphRectangle.Bottom,
                                Right = l.GlyphRectangle.Right
                            }
                        }).ToList(),
                        Bounds = new PDFSelection
                        {
                            Top = word.BoundingBox.Left,
                            Left = word.BoundingBox.Top,
                            Bottom = word.BoundingBox.Bottom,
                            Right = word.BoundingBox.Right
                        },
                        Centroid = new PDFCentroid
                        {
                            X = word.BoundingBox.Centroid.X,
                            Y = word.BoundingBox.Centroid.Y,
                        }
                    });
                }
            }
            return wordList;
        }

        /// <summary>
        /// Function to approximate words to lines based on centroid approximation
        /// </summary>
        /// <param name="words"></param>
        /// <param name="approximationThreashold"></param>
        /// <returns></returns>
        public List<PDFLine> ApproximateAllLines(List<PDFWord> words, double approximationThreashold = 2)
        {
            var lines = new List<PDFLine>();
            var clusters = GetLineClusters(words, approximationThreashold).Reverse();
            //ReOrder by location
            foreach (var cluster in clusters)
            {
                var reOrderedWords = cluster.OrderBy(x => x.Centroid.X);
                lines.Add(new PDFLine
                {
                    Name = string.Join(' ', reOrderedWords.Select(x => x.Name)),
                    Words = reOrderedWords.ToList(),
                    Centroid = new PDFCentroid
                    {
                        X = reOrderedWords.Average(x => x.Centroid.X),
                        Y = reOrderedWords.Average(x => x.Centroid.Y)
                    }
                });
            }
            return lines;
        }

        /// <summary>
        /// Function to approximate lines to text-blocks based on centroid approximation
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="approximationThreashold"></param>
        /// <returns></returns>
        public List<PDFTextBlock> ApproximateAllTextBlocks(List<PDFLine> lines, double approximationThreashold = 15)
        {
            var blocks = new List<PDFTextBlock>();
            var clusters = GetBlockClusters(lines, approximationThreashold).Reverse();
            foreach (var block in clusters)
            {
                var formattedBlock = block.Reverse();
                blocks.Add(new PDFTextBlock
                {
                    Lines = block.ToList(),
                    Name = string.Join('\n', formattedBlock.Select(x => x.Name)),
                    Centroid = new PDFCentroid
                    {
                        X = formattedBlock.Average(x => x.Centroid.X),
                        Y = formattedBlock.Average(x => x.Centroid.Y)
                    },
                });
            }
            return blocks;
        }

        /// <summary>
        /// Function to remove specific pages from a PDF page collection
        /// </summary>
        /// <param name="pageCollection"></param>
        /// <param name="pageRange"></param>
        /// <returns></returns>
        public PDFPageCollection RemovePages(PDFPageCollection pageCollection, PageRange pageRange)
        {
            PdfDocument PDFDoc = PdfReader.Open(pageCollection.PDFFile.Stream, PdfDocumentOpenMode.Import);
            PdfDocument PDFNewDoc = new PdfDocument();
            for (int i = 0; i < PDFDoc.Pages.Count; i++)
            {
                if (!pageRange.PageNumbers.Contains(i + 1))
                {
                    PDFNewDoc.AddPage(PDFDoc.Pages[i]);
                }
            }
            var newPDFFile = RebuildPDFFile(PDFNewDoc, pageCollection.PDFFile);
            return new PDFPageCollection
            {
                PDFFile = pageCollection.PDFFile,
                CollectionsPDFFile = newPDFFile,
                Pages = newPDFFile.Pages
            };
        }

        /// <summary>
        /// Function to save the PDF to a filesystem
        /// </summary>
        /// <param name="pdfFile"></param>
        /// <param name="destinationLocation"></param>
        public void SavePDF(PDFFile pdfFile, string destinationLocation)
        {
            pdfFile.Document.Save(destinationLocation);
        }

        /// <summary>
        /// Split PDF file based on file size
        /// </summary>
        /// <param name="pdfFile"></param>
        /// <param name="byteLimit"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public List<PDFFile> SplitPDFByFileSize(PDFFile pdfFile, string outputLoc, long limit)
        {
            var pdfFiles = new List<PDFFile>();
            limit = limit * 1000;
            PdfDocument input = PdfReader.Open(pdfFile.Stream, PdfDocumentOpenMode.Import);
            PdfDocument output = new PdfDocument();
            int j = 1;

            var backupStream = new MemoryStream();
            var temp = new MemoryStream();

            for (int i = 0; i < input.PageCount; i++)
            {
                PdfPage page = input.Pages[i];
                output.AddPage(page);
                output.Save(temp);

                if (temp.Length < limit)
                {
                    output.Save(backupStream);
                }

                if (temp.Length > limit)
                {
                    if (output.PageCount > 1)
                    {
                        var newPDFFile = RebuildPDFFile(output, pdfFile);
                        newPDFFile.FileName = "chunk-" + j;
                        newPDFFile.Stream = backupStream;
                        newPDFFile.Size = backupStream.Length;
                        newPDFFile.DirectoryName = outputLoc;
                        newPDFFile.FileSize = backupStream.Length.Bytes().Humanize();
                        pdfFiles.Add(newPDFFile);

                        GC.Collect();

                        output = new PdfDocument();
                        temp = new MemoryStream();
                        backupStream = new MemoryStream();

                        ++j; i--;
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            }
            GC.Collect();
            return pdfFiles;
        }
        #endregion PUBLIC FUNCTIONS



        #region PRIVATE FUNCTIONS
        /// <summary>
        /// Function to get a line cluster
        /// </summary>
        /// <param name="data"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        private IEnumerable<IEnumerable<PDFWord>> GetLineClusters(List<PDFWord> data, double delta = 1.0)
        {
            var cluster = new List<PDFWord>();
            foreach (var item in data.OrderBy(x => x.Centroid.Y))
            {
                if (cluster.Count > 0 && item.Centroid.Y > cluster[cluster.Count - 1].Centroid.Y + delta)
                {
                    yield return cluster;
                    cluster = new List<PDFWord>();
                }
                cluster.Add(item);
            }
            if (cluster.Count > 0)
                yield return cluster;
        }

        /// <summary>
        /// Function to get a Block Cluster
        /// </summary>
        /// <param name="data"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        private IEnumerable<IEnumerable<PDFLine>> GetBlockClusters(List<PDFLine> data, double delta = 3.0)
        {
            var cluster = new List<PDFLine>();
            foreach (var item in data.OrderBy(x => x.Centroid.Y))
            {
                if (cluster.Count > 0 && item.Centroid.Y > cluster[cluster.Count - 1].Centroid.Y + delta)
                {
                    yield return cluster;
                    cluster = new List<PDFLine>();
                }
                cluster.Add(item);
            }
            if (cluster.Count > 0)
                yield return cluster;
        }


        /// <summary>
        /// Helper function to rebuild PDFFile
        /// </summary>
        /// <param name="newPDFDoc"></param>
        /// <param name="oldPDFFile"></param>
        /// <returns></returns>
        private PDFFile RebuildPDFFile(PdfDocument newDocument, PDFFile oldPDFFile)
        {
            //Generate Page Info
            var pages = new List<PDFPage>();
            for (var i = 1; i <= newDocument.Pages.Count; i++)
            {
                var page = new PDFPage
                {
                    PageNumber = i
                };
                pages.Add(page);
            }
            //Return           
            var newPdf = new PDFFile
            {
                DirectoryName = oldPDFFile.DirectoryName,
                Document = newDocument,
                FileName = oldPDFFile.FileName,
                Pages = pages,
                TotalPages = pages.Count,
                Stream = new MemoryStream()
            };
            newDocument.Save(newPdf.Stream);
            return newPdf;
        }
        #endregion PRIVATE FUNCTIONS
    }
}

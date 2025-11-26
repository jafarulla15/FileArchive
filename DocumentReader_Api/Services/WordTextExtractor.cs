using NPOI.XWPF.UserModel;   // for .docx
// using NPOI.HWPF;                 // for .doc
// using NPOI.HWPF.Extractor;       // for .doc
using System.Text;

namespace PdfReaderApi.Services
{
    public class WordTextExtractor : IWordTextExtractor
    {
        public Task<string> ExtractTextAsync(Stream wordStream, string extension)
        {
            if (wordStream == null || !wordStream.CanRead)
                throw new ArgumentException("Invalid Word document stream.");

            if (wordStream.CanSeek)
                wordStream.Seek(0, SeekOrigin.Begin);

            extension = extension?.ToLowerInvariant();

            if (extension != ".docx")
            {
                throw new NotSupportedException("Only .docx files are supported in this API.");
            }

            return Task.FromResult(ExtractDocx(wordStream));
        }

        private string ExtractDocx(Stream stream)
        {
            var sb = new StringBuilder();

            using (var doc = new XWPFDocument(stream))
            {
                // Paragraphs
                foreach (var para in doc.Paragraphs)
                {
                    sb.AppendLine(para.ParagraphText);
                }

                // Tables (optional, but useful)
                foreach (var table in doc.Tables)
                {
                    foreach (var row in table.Rows)
                    {
                        foreach (var cell in row.GetTableCells())
                        {
                            sb.Append(cell.GetText());
                            sb.Append('\t');
                        }
                        sb.AppendLine();
                    }
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }
    }
}

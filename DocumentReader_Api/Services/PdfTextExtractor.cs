using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace PdfReaderApi.Services
{
    public class PdfTextExtractor : IPdfTextExtractor
    {
        public Task<string> ExtractTextAsync(Stream pdfStream)
        {
            if (pdfStream == null || !pdfStream.CanRead)
                throw new ArgumentException("Invalid PDF stream.");

            // We need to ensure the stream position is at the beginning.
            if (pdfStream.CanSeek)
            {
                pdfStream.Seek(0, SeekOrigin.Begin);
            }

            var textBuilder = new System.Text.StringBuilder();

            using (var pdf = PdfDocument.Open(pdfStream))
            {
                foreach (Page page in pdf.GetPages())
                {
                    string pageText = page.Text;
                    textBuilder.AppendLine(pageText);
                    textBuilder.AppendLine(); // page separator
                }
            }

            return Task.FromResult(textBuilder.ToString());
        }
    }
}

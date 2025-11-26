namespace PdfReaderApi.Services
{
    public interface IPdfTextExtractor
    {
        /// <summary>
        /// Extracts all text from the given PDF stream.
        /// </summary>
        /// <param name="pdfStream">Stream containing the PDF file.</param>
        /// <returns>Extracted text as a single string.</returns>
        Task<string> ExtractTextAsync(Stream pdfStream);
    }
}

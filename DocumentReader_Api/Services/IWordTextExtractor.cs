namespace PdfReaderApi.Services
{
    public interface IWordTextExtractor
    {
        /// <summary>
        /// Extracts text from a Word (.doc / .docx) document.
        /// </summary>
        /// <param name="wordStream">Stream of the Word file.</param>
        /// <param name="extension">File extension, e.g. ".doc" or ".docx".</param>
        /// <returns>Extracted text.</returns>
        Task<string> ExtractTextAsync(Stream wordStream, string extension);
    }
}

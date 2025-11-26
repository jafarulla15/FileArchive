using Microsoft.AspNetCore.Mvc;
using PdfReaderApi.Services;

namespace PdfReaderApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PdfController : ControllerBase
    {
        private readonly IPdfTextExtractor _pdfTextExtractor;
        private readonly long _maxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

        public PdfController(IPdfTextExtractor pdfTextExtractor)
        {
            _pdfTextExtractor = pdfTextExtractor;
        }

        /// <summary>
        /// Upload a PDF file and get its extracted text.
        /// </summary>
        /// <param name="file">PDF file as multipart/form-data.</param>
        /// <returns>Extracted text.</returns>
        [HttpPost("read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
        public async Task<IActionResult> ReadPdf([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(file.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only PDF files are allowed.");
            }

            if (file.Length > _maxFileSizeBytes)
            {
                return StatusCode(StatusCodes.Status413PayloadTooLarge,
                    $"File size exceeds limit of {_maxFileSizeBytes / (1024 * 1024)} MB.");
            }

            try
            {
                await using var stream = file.OpenReadStream();
                var text = await _pdfTextExtractor.ExtractTextAsync(stream);

                // You can return plain text or JSON. Here we wrap it in JSON.
                return Ok(new
                {
                    fileName = file.FileName,
                    length = file.Length,
                    text
                });
            }
            catch (Exception ex)
            {
                // In production, log the exception instead of returning the full message.
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occurred while processing the PDF: {ex.Message}");
            }
        }

        /// <summary>
        /// Simple test endpoint.
        /// </summary>
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("PDF Reader API is up and running.");
        }
    }
}

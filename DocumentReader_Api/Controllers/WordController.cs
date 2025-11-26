using Microsoft.AspNetCore.Mvc;
using PdfReaderApi.Services;

namespace PdfReaderApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WordController : ControllerBase
    {
        private readonly IWordTextExtractor _wordTextExtractor;
        private readonly long _maxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

        public WordController(IWordTextExtractor wordTextExtractor)
        {
            _wordTextExtractor = wordTextExtractor;
        }

        /// <summary>
        /// Upload a Word document (.doc / .docx) and get extracted text.
        /// </summary>
        [HttpPost("read")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
        public async Task<IActionResult> ReadWord([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var extension = Path.GetExtension(file.FileName);

            if (!string.Equals(extension, ".doc", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(extension, ".docx", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only .doc or .docx files are allowed.");
            }

            if (file.Length > _maxFileSizeBytes)
            {
                return StatusCode(StatusCodes.Status413PayloadTooLarge,
                    $"File size exceeds limit of {_maxFileSizeBytes / (1024 * 1024)} MB.");
            }

            try
            {
                await using var stream = file.OpenReadStream();
                var text = await _wordTextExtractor.ExtractTextAsync(stream, extension);

                return Ok(new
                {
                    fileName = file.FileName,
                    length = file.Length,
                    text
                });
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occurred while processing the Word file: {ex.Message}");
            }
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("Word Reader API is up and running.");
        }
    }
}

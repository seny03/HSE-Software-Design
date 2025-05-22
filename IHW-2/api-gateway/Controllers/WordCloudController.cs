using ApiGateway.Models;
using ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WordCloudController : ControllerBase
    {
        private readonly IAnalysisServiceClient _analysisServiceClient;
        private readonly ILogger<WordCloudController> _logger;

        public WordCloudController(
            IAnalysisServiceClient analysisServiceClient,
            ILogger<WordCloudController> logger)
        {
            _analysisServiceClient = analysisServiceClient;
            _logger = logger;
        }

        /// <summary>
        /// Generate a word cloud for a file
        /// </summary>
        /// <param name="fileId">File ID</param>
        /// <returns>Word cloud image URL</returns>
        [HttpGet("{fileId}")]
        [ProducesResponseType(typeof(WordCloudResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GenerateWordCloud(string fileId)
        {
            try
            {
                _logger.LogInformation("Generating word cloud for file: {FileId}", fileId);
                var wordCloud = await _analysisServiceClient.GenerateWordCloudAsync(fileId);
                if (wordCloud == null)
                    return NotFound(new ErrorResponse { Error = $"Word cloud for file {fileId} not found" });
                return Ok(wordCloud);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new ErrorResponse { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating word cloud for file: {FileId}", fileId);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new ErrorResponse { Error = $"Error generating word cloud for file with ID {fileId}" });
            }
        }
    }
}

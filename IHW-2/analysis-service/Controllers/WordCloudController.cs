using AnalysisService.Models;
using AnalysisService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AnalysisService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WordCloudController : ControllerBase
    {
        private readonly IWordCloudService _wordCloudService;
        private readonly IFileClientService _fileClientService;
        private readonly ILogger<WordCloudController> _logger;

        public WordCloudController(
            IWordCloudService wordCloudService,
            IFileClientService fileClientService,
            ILogger<WordCloudController> logger)
        {
            _wordCloudService = wordCloudService;
            _fileClientService = fileClientService;
            _logger = logger;
        }

        /// <summary>
        /// Generate a word cloud for a file
        /// </summary>
        /// <param name="fileId">File ID</param>
        /// <returns>Word cloud URL</returns>
        [HttpGet("{fileId}")]
        [ProducesResponseType(typeof(WordCloudResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GenerateWordCloud(string fileId)
        {
            try
            {
                if (string.IsNullOrEmpty(fileId))
                {
                    return BadRequest(new ErrorResponse { Error = "File ID is required" });
                }

                _logger.LogInformation("Generating word cloud for file: {FileId}", fileId);
                
                // Get file from File Service
                var file = await _fileClientService.GetFileByIdAsync(fileId);
                
                // Generate word cloud URL
                var wordCloudUrl = await _wordCloudService.GetOrGenerateWordCloudUrlAsync(fileId, file.Content);
                
                if (string.IsNullOrEmpty(wordCloudUrl))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, 
                        new ErrorResponse { Error = "Error generating word cloud" });
                }
                
                return Ok(new WordCloudResponse { WordCloudUrl = wordCloudUrl });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new ErrorResponse { Error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(new ErrorResponse { Error = ex.Message });
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

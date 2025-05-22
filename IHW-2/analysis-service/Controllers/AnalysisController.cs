using AnalysisService.Models;
using AnalysisService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AnalysisService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnalysisController : ControllerBase
    {
        private readonly ITextAnalysisService _textAnalysisService;
        private readonly IFileClientService _fileClientService;
        private readonly ILogger<AnalysisController> _logger;

        public AnalysisController(
            ITextAnalysisService textAnalysisService,
            IFileClientService fileClientService,
            ILogger<AnalysisController> logger)
        {
            _textAnalysisService = textAnalysisService;
            _fileClientService = fileClientService;
            _logger = logger;
        }

        /// <summary>
        /// Request analysis for a file
        /// </summary>
        /// <param name="request">Analysis request with file ID</param>
        /// <returns>Analysis result</returns>
        [HttpPost]
        [ProducesResponseType(typeof(AnalysisResult), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RequestAnalysis(AnalysisRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.FileId))
                {
                    return BadRequest(new ErrorResponse { Error = "File ID is required" });
                }

                _logger.LogInformation("Requesting analysis for file: {FileId}", request.FileId);
                
                // Get file from File Service
                var file = await _fileClientService.GetFileByIdAsync(request.FileId);
                
                // Analyze text
                var result = await _textAnalysisService.AnalyzeTextAsync(request.FileId, file.Content);
                
                return CreatedAtAction(nameof(GetAnalysis), new { id = result.AnalysisId }, result);
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
                _logger.LogError(ex, "Error analyzing file: {FileId}", request.FileId);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new ErrorResponse { Error = "Error analyzing file" });
            }
        }

        /// <summary>
        /// Get analysis by ID
        /// </summary>
        /// <param name="id">Analysis ID</param>
        /// <returns>Analysis result</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AnalysisResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAnalysis(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var analysisId))
                {
                    return BadRequest(new ErrorResponse { Error = "Invalid analysis ID format" });
                }

                _logger.LogInformation("Getting analysis by ID: {AnalysisId}", id);
                var analysis = await _textAnalysisService.GetAnalysisAsync(analysisId);
                
                if (analysis == null)
                {
                    return NotFound(new ErrorResponse { Error = $"Analysis with ID {id} not found" });
                }
                
                return Ok(analysis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving analysis: {AnalysisId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new ErrorResponse { Error = $"Error retrieving analysis with ID {id}" });
            }
        }
    }
}

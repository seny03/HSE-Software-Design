using AnalysisService.Models;
using AnalysisService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AnalysisService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompareController : ControllerBase
    {
        private readonly ITextAnalysisService _textAnalysisService;
        private readonly IFileClientService _fileClientService;
        private readonly ILogger<CompareController> _logger;

        public CompareController(
            ITextAnalysisService textAnalysisService,
            IFileClientService fileClientService,
            ILogger<CompareController> logger)
        {
            _textAnalysisService = textAnalysisService;
            _fileClientService = fileClientService;
            _logger = logger;
        }

        /// <summary>
        /// Compare files for plagiarism
        /// </summary>
        /// <param name="request">File IDs to compare</param>
        /// <returns>Comparison results</returns>
        [HttpPost]
        [ProducesResponseType(typeof(List<ComparisonResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CompareFiles(ComparisonRequest request)
        {
            try
            {
                if (request?.FileIds == null || request.FileIds.Count < 2)
                {
                    return BadRequest(new ErrorResponse { Error = "At least two file IDs are required" });
                }

                _logger.LogInformation("Comparing files: {FileIds}", string.Join(", ", request.FileIds));
                
                // Get file contents from File Service
                var fileContents = await _fileClientService.GetFileContentsAsync(request.FileIds);
                
                // Compare texts
                var results = await _textAnalysisService.CompareTextsAsync(request.FileIds, fileContents);
                
                return Ok(results);
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
                _logger.LogError(ex, "Error comparing files: {FileIds}", 
                    request?.FileIds != null ? string.Join(", ", request.FileIds) : "null");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new ErrorResponse { Error = "Error comparing files" });
            }
        }
    }
}

using ApiGateway.Models;
using ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompareController : ControllerBase
    {
        private readonly IAnalysisServiceClient _analysisServiceClient;
        private readonly ILogger<CompareController> _logger;

        public CompareController(
            IAnalysisServiceClient analysisServiceClient,
            ILogger<CompareController> logger)
        {
            _analysisServiceClient = analysisServiceClient;
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
            if (request?.FileIds == null || request.FileIds.Count < 2)
            {
                return BadRequest(new ErrorResponse { Error = "At least two file IDs are required" });
            }

            try
            {
                _logger.LogInformation("Comparing files: {FileIds}", string.Join(", ", request.FileIds));
                var results = await _analysisServiceClient.CompareFilesAsync(request.FileIds);
                return Ok(results);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new ErrorResponse { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error comparing files: {FileIds}", string.Join(", ", request.FileIds));
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new ErrorResponse { Error = "Error comparing files" });
            }
        }
    }
}

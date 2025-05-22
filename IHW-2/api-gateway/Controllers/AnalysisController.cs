using ApiGateway.Models;
using ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnalysisController : ControllerBase
    {
        private readonly IAnalysisServiceClient _analysisServiceClient;
        private readonly ILogger<AnalysisController> _logger;

        public AnalysisController(
            IAnalysisServiceClient analysisServiceClient,
            ILogger<AnalysisController> logger)
        {
            _analysisServiceClient = analysisServiceClient;
            _logger = logger;
        }

        /// <summary>
        /// Get analysis results by ID
        /// </summary>
        /// <param name="id">Analysis ID</param>
        /// <returns>Analysis results</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AnalysisResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAnalysisById(string id)
        {
            try
            {
                _logger.LogInformation("Getting analysis by ID: {AnalysisId}", id);
                var analysis = await _analysisServiceClient.GetAnalysisResultAsync(id);
                if (analysis == null)
                    return NotFound(new ErrorResponse { Error = $"Analysis with ID {id} not found" });
                return Ok(analysis);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new ErrorResponse { Error = ex.Message });
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

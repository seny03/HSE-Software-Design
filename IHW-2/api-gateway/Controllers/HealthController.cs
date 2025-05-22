using ApiGateway.Models;
using ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly IFileServiceClient _fileServiceClient;
        private readonly IAnalysisServiceClient _analysisServiceClient;
        private readonly ILogger<HealthController> _logger;

        public HealthController(
            IFileServiceClient fileServiceClient, 
            IAnalysisServiceClient analysisServiceClient,
            ILogger<HealthController> logger)
        {
            _fileServiceClient = fileServiceClient;
            _analysisServiceClient = analysisServiceClient;
            _logger = logger;
        }

        /// <summary>
        /// Check the health of the API and connected services
        /// </summary>
        /// <returns>Health status of the API and services</returns>
        [HttpGet]
        [ProducesResponseType(typeof(GatewayHealthStatus), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHealthStatus()
        {
            _logger.LogInformation("Health check requested");

            ServiceHealthStatus fileServiceHealth;
            ServiceHealthStatus analysisServiceHealth;

            try
            {
                fileServiceHealth = await _fileServiceClient.CheckHealthAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking File Service health");
                fileServiceHealth = new ServiceHealthStatus { Status = "down" };
            }

            try
            {
                analysisServiceHealth = await _analysisServiceClient.CheckHealthAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking Analysis Service health");
                analysisServiceHealth = new ServiceHealthStatus { Status = "down" };
            }

            var healthStatus = new GatewayHealthStatus
            {
                Status = "up",
                Services = new Dictionary<string, string>
                {
                    { "fileService", fileServiceHealth.Status },
                    { "analysisService", analysisServiceHealth.Status }
                }
            };

            return Ok(healthStatus);
        }
    }
}

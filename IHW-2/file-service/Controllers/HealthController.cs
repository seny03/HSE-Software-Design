using FileService.Models;
using Microsoft.AspNetCore.Mvc;

namespace FileService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Check the health of the service
        /// </summary>
        /// <returns>Service health status</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ServiceHealthStatus), StatusCodes.Status200OK)]
        public IActionResult GetHealth()
        {
            _logger.LogInformation("Health check requested");
            return Ok(new ServiceHealthStatus { Status = "up" });
        }
    }
}

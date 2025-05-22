using AnalysisService.Controllers;
using AnalysisService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AnalysisService.Tests.Controllers
{
    public class HealthControllerTests
    {
        [Fact]
        public void GetHealth_ReturnsOkWithStatusUp()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<HealthController>>();
            var controller = new HealthController(mockLogger.Object);

            // Act
            var result = controller.GetHealth();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var status = Assert.IsType<ServiceHealthStatus>(okResult.Value);
            Assert.Equal("up", status.Status);
        }
    }
}

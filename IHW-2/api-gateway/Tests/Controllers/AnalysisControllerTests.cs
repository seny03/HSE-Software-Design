using ApiGateway.Controllers;
using ApiGateway.Models;
using ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ApiGateway.Tests.Controllers
{
    public class AnalysisControllerTests
    {
        private readonly Mock<IAnalysisServiceClient> _mockAnalysisClient = new();
        private readonly ILogger<AnalysisController> _logger = Mock.Of<ILogger<AnalysisController>>();

        [Fact]
        public async Task GetAnalysis_ReturnsOk_WhenAnalysisFound()
        {
            // Arrange
            var analysisId = Guid.NewGuid().ToString();
            var analysisResult = new AnalysisResult { AnalysisId = analysisId };
            _mockAnalysisClient.Setup(c => c.GetAnalysisResultAsync(analysisId))
                .ReturnsAsync(analysisResult);

            var controller = new AnalysisController(_mockAnalysisClient.Object, _logger);

            // Act
            var result = await controller.GetAnalysisById(analysisId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<AnalysisResult>(okResult.Value);
            Assert.Equal(analysisId, value.AnalysisId);
        }

        [Fact]
        public async Task GetAnalysis_ReturnsNotFound_WhenNull()
        {
            // Arrange
            var analysisId = Guid.NewGuid().ToString();
            _mockAnalysisClient.Setup(c => c.GetAnalysisResultAsync(analysisId))
                .ReturnsAsync((AnalysisResult)null!);

            var controller = new AnalysisController(_mockAnalysisClient.Object, _logger);

            // Act
            var result = await controller.GetAnalysisById(analysisId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}

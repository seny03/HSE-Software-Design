using ApiGateway.Controllers;
using ApiGateway.Models;
using ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ApiGateway.Tests.Controllers
{
    public class CompareControllerTests
    {
        private readonly Mock<IAnalysisServiceClient> _mockAnalysisClient = new();
        private readonly ILogger<CompareController> _logger = Mock.Of<ILogger<CompareController>>();

        [Fact]
        public async Task CompareFiles_ReturnsOk_WhenValid()
        {
            // Arrange
            var fileIds = new List<string> { "id1", "id2" };
            var request = new ComparisonRequest { FileIds = fileIds };
            var compareResults = new List<ComparisonResult>
            {
                new ComparisonResult { FileId1 = "id1", FileId2 = "id2", SimilarityScore = 0.9 }
            };

            _mockAnalysisClient.Setup(c => c.CompareFilesAsync(fileIds)).ReturnsAsync(compareResults);

            var controller = new CompareController(_mockAnalysisClient.Object, _logger);

            // Act
            var result = await controller.CompareFiles(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<List<ComparisonResult>>(okResult.Value);
            Assert.Single(value);
        }

        [Fact]
        public async Task CompareFiles_ReturnsBadRequest_WhenException()
        {
            // Arrange
            var fileIds = new List<string> { "id1" }; // not enough for comparison
            var request = new ComparisonRequest { FileIds = fileIds };

            _mockAnalysisClient.Setup(c => c.CompareFilesAsync(fileIds)).ThrowsAsync(new System.Exception("error"));

            var controller = new CompareController(_mockAnalysisClient.Object, _logger);

            // Act
            var result = await controller.CompareFiles(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}

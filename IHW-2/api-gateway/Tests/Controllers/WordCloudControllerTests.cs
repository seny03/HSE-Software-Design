using ApiGateway.Controllers;
using ApiGateway.Models;
using ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace ApiGateway.Tests.Controllers
{
    public class WordCloudControllerTests
    {
        private readonly Mock<IAnalysisServiceClient> _mockAnalysisClient = new();
        private readonly ILogger<WordCloudController> _logger = Mock.Of<ILogger<WordCloudController>>();

        [Fact]
        public async Task GenerateWordCloud_ReturnsOk_WhenFound()
        {
            // Arrange
            var fileId = "file-id";
            var url = "https://cloud.url/image.png";
            _mockAnalysisClient.Setup(c => c.GenerateWordCloudAsync(fileId))
                .ReturnsAsync(new WordCloudResponse { WordCloudUrl = url });

            var controller = new WordCloudController(_mockAnalysisClient.Object, _logger);

            // Act
            var result = await controller.GenerateWordCloud(fileId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<WordCloudResponse>(okResult.Value);
            Assert.Equal(url, value.WordCloudUrl);
        }

        [Fact]
        public async Task GenerateWordCloud_ReturnsNotFound_WhenNull()
        {
            // Arrange
            var fileId = "file-id";
            _mockAnalysisClient.Setup(c => c.GenerateWordCloudAsync(fileId))
                .ReturnsAsync((WordCloudResponse)null!);

            var controller = new WordCloudController(_mockAnalysisClient.Object, _logger);

            // Act
            var result = await controller.GenerateWordCloud(fileId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}

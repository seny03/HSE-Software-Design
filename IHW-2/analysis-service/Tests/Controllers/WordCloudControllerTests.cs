using AnalysisService.Controllers;
using AnalysisService.Models;
using AnalysisService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AnalysisService.Tests.Controllers
{
    public class WordCloudControllerTests
    {
        private readonly Mock<IWordCloudService> _mockWordCloudService = new();
        private readonly Mock<IFileClientService> _mockFileClientService = new();
        private readonly Mock<ILogger<WordCloudController>> _mockLogger = new();

        [Fact]
        public async Task GenerateWordCloud_ReturnsBadRequest_IfFileIdIsNullOrEmpty()
        {
            // Arrange
            var mockWordCloudService = new Mock<IWordCloudService>();
            var mockFileClientService = new Mock<IFileClientService>();
            var mockLogger = new Mock<ILogger<WordCloudController>>();

            var controller = new WordCloudController(mockWordCloudService.Object, mockFileClientService.Object, mockLogger.Object);

            // Act
            var result = await controller.GenerateWordCloud("");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            Assert.Equal("File ID is required", errorResponse.Error);
        }

        [Fact]
        public async Task GenerateWordCloud_ReturnsNotFound_OnKeyNotFound()
        {
            var fileId = "123";
            _mockFileClientService.Setup(x => x.GetFileByIdAsync(fileId)).ThrowsAsync(new KeyNotFoundException("Not found"));

            var controller = new WordCloudController(_mockWordCloudService.Object, _mockFileClientService.Object, _mockLogger.Object);

            var result = await controller.GenerateWordCloud(fileId);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GenerateWordCloud_ReturnsBadRequest_OnArgumentException()
        {
            var fileId = "123";
            _mockFileClientService.Setup(x => x.GetFileByIdAsync(fileId)).ThrowsAsync(new ArgumentException("Bad argument"));

            var controller = new WordCloudController(_mockWordCloudService.Object, _mockFileClientService.Object, _mockLogger.Object);

            var result = await controller.GenerateWordCloud(fileId);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GenerateWordCloud_ReturnsInternalServerError_OnUnknownException()
        {
            var fileId = "123";
            _mockFileClientService.Setup(x => x.GetFileByIdAsync(fileId)).ThrowsAsync(new Exception("Unknown"));

            var controller = new WordCloudController(_mockWordCloudService.Object, _mockFileClientService.Object, _mockLogger.Object);

            var result = await controller.GenerateWordCloud(fileId);

            var objResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objResult.StatusCode);
        }

        [Fact]
        public async Task GenerateWordCloud_ReturnsOk_WhenEverythingIsFine()
        {
            var fileId = "abc";
            var fileContent = "some text";
            _mockFileClientService.Setup(x => x.GetFileByIdAsync(fileId)).ReturnsAsync(new FileDto { Content = fileContent });
            _mockWordCloudService.Setup(x => x.GetOrGenerateWordCloudUrlAsync(fileId, fileContent)).ReturnsAsync("http://wordcloud");

            var controller = new WordCloudController(_mockWordCloudService.Object, _mockFileClientService.Object, _mockLogger.Object);

            var result = await controller.GenerateWordCloud(fileId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var resp = Assert.IsType<WordCloudResponse>(okResult.Value);
            Assert.Equal("http://wordcloud", resp.WordCloudUrl);
        }
    }
}

using AnalysisService.Controllers;
using AnalysisService.Models;
using AnalysisService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AnalysisService.Tests.Controllers
{
    public class AnalysisControllerTests
    {
        private readonly Mock<ITextAnalysisService> _mockTextAnalysisService;
        private readonly Mock<IFileClientService> _mockFileClientService;
        private readonly Mock<ILogger<AnalysisController>> _mockLogger;
        private readonly AnalysisController _controller;

        public AnalysisControllerTests()
        {
            _mockTextAnalysisService = new Mock<ITextAnalysisService>();
            _mockFileClientService = new Mock<IFileClientService>();
            _mockLogger = new Mock<ILogger<AnalysisController>>();
            _controller = new AnalysisController(_mockTextAnalysisService.Object, _mockFileClientService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task RequestAnalysis_ReturnsCreatedResult_WhenAnalysisSucceeds()
        {
            // Arrange
            var request = new AnalysisRequest { FileId = "123" };
            var file = new FileDto
            {
                Id = "123",
                Filename = "test.txt",
                Content = "This is test content",
                Size = 100,
                CreatedAt = DateTime.UtcNow
            };
            
            var analysisResult = new AnalysisResult
            {
                AnalysisId = "abc",
                FileId = "123",
                ParagraphCount = 1,
                WordCount = 4,
                CharacterCount = 19,
                CreatedAt = DateTime.UtcNow
            };
            
            _mockFileClientService.Setup(x => x.GetFileByIdAsync(request.FileId))
                .ReturnsAsync(file);
                
            _mockTextAnalysisService.Setup(x => x.AnalyzeTextAsync(request.FileId, file.Content))
                .ReturnsAsync(analysisResult);

            // Act
            var result = await _controller.RequestAnalysis(request);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(AnalysisController.GetAnalysis), createdResult.ActionName);
            Assert.Equal("abc", createdResult.RouteValues["id"]);
            
            var returnValue = Assert.IsType<AnalysisResult>(createdResult.Value);
            Assert.Equal("abc", returnValue.AnalysisId);
            Assert.Equal("123", returnValue.FileId);
            Assert.Equal(1, returnValue.ParagraphCount);
            Assert.Equal(4, returnValue.WordCount);
            Assert.Equal(19, returnValue.CharacterCount);
        }

        [Fact]
        public async Task RequestAnalysis_ReturnsBadRequest_WhenFileIdIsEmpty()
        {
            // Arrange
            var request = new AnalysisRequest { FileId = "" };

            // Act
            var result = await _controller.RequestAnalysis(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            Assert.Equal("File ID is required", errorResponse.Error);
        }

        [Fact]
        public async Task RequestAnalysis_ReturnsNotFound_WhenFileDoesNotExist()
        {
            // Arrange
            var request = new AnalysisRequest { FileId = "nonexistent" };
            
            _mockFileClientService.Setup(x => x.GetFileByIdAsync(request.FileId))
                .ThrowsAsync(new KeyNotFoundException($"File with ID {request.FileId} not found"));

            // Act
            var result = await _controller.RequestAnalysis(request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(notFoundResult.Value);
            Assert.Contains(request.FileId, errorResponse.Error);
        }

        [Fact]
        public async Task RequestAnalysis_ReturnsBadRequest_WhenArgumentExceptionThrown()
        {
            // Arrange
            var request = new AnalysisRequest { FileId = "123" };
            var file = new FileDto
            {
                Id = "123",
                Filename = "test.txt",
                Content = "This is test content",
                Size = 100,
                CreatedAt = DateTime.UtcNow
            };
            
            _mockFileClientService.Setup(x => x.GetFileByIdAsync(request.FileId))
                .ReturnsAsync(file);
                
            _mockTextAnalysisService.Setup(x => x.AnalyzeTextAsync(request.FileId, file.Content))
                .ThrowsAsync(new ArgumentException("Invalid file ID format"));

            // Act
            var result = await _controller.RequestAnalysis(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            Assert.Equal("Invalid file ID format", errorResponse.Error);
        }

        [Fact]
        public async Task RequestAnalysis_ReturnsServerError_WhenUnexpectedExceptionThrown()
        {
            // Arrange
            var request = new AnalysisRequest { FileId = "123" };
            var file = new FileDto
            {
                Id = "123",
                Filename = "test.txt",
                Content = "This is test content",
                Size = 100,
                CreatedAt = DateTime.UtcNow
            };
            
            _mockFileClientService.Setup(x => x.GetFileByIdAsync(request.FileId))
                .ReturnsAsync(file);
                
            _mockTextAnalysisService.Setup(x => x.AnalyzeTextAsync(request.FileId, file.Content))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.RequestAnalysis(request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var errorResponse = Assert.IsType<ErrorResponse>(statusCodeResult.Value);
            Assert.Equal("Error analyzing file", errorResponse.Error);
        }

        [Fact]
        public async Task GetAnalysis_ReturnsOk_WhenAnalysisExists()
        {
            // Arrange
            var analysisId = Guid.NewGuid();
            var analysisResult = new AnalysisResult
            {
                AnalysisId = analysisId.ToString(),
                FileId = "123",
                ParagraphCount = 1,
                WordCount = 4,
                CharacterCount = 19,
                CreatedAt = DateTime.UtcNow
            };
            
            _mockTextAnalysisService.Setup(x => x.GetAnalysisAsync(analysisId))
                .ReturnsAsync(analysisResult);

            // Act
            var result = await _controller.GetAnalysis(analysisId.ToString());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<AnalysisResult>(okResult.Value);
            Assert.Equal(analysisId.ToString(), returnValue.AnalysisId);
            Assert.Equal("123", returnValue.FileId);
            Assert.Equal(1, returnValue.ParagraphCount);
            Assert.Equal(4, returnValue.WordCount);
            Assert.Equal(19, returnValue.CharacterCount);
        }

        [Fact]
        public async Task GetAnalysis_ReturnsBadRequest_WhenIdFormatIsInvalid()
        {
            // Act
            var result = await _controller.GetAnalysis("invalid-guid");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            Assert.Equal("Invalid analysis ID format", errorResponse.Error);
        }

        [Fact]
        public async Task GetAnalysis_ReturnsNotFound_WhenAnalysisDoesNotExist()
        {
            // Arrange
            var analysisId = Guid.NewGuid();
            
            _mockTextAnalysisService.Setup(x => x.GetAnalysisAsync(analysisId))
                .ReturnsAsync((AnalysisResult)null);

            // Act
            var result = await _controller.GetAnalysis(analysisId.ToString());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(notFoundResult.Value);
            Assert.Contains(analysisId.ToString(), errorResponse.Error);
        }

        [Fact]
        public async Task GetAnalysis_ReturnsServerError_WhenUnexpectedExceptionThrown()
        {
            // Arrange
            var analysisId = Guid.NewGuid();
            
            _mockTextAnalysisService.Setup(x => x.GetAnalysisAsync(analysisId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetAnalysis(analysisId.ToString());

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var errorResponse = Assert.IsType<ErrorResponse>(statusCodeResult.Value);
            Assert.Contains(analysisId.ToString(), errorResponse.Error);
        }
    }
}

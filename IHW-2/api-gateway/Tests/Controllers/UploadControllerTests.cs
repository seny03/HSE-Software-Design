using ApiGateway.Controllers;
using ApiGateway.Models;
using ApiGateway.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace ApiGateway.Tests.Controllers
{
    public class UploadControllerTests
    {
        private readonly Mock<IFileServiceClient> _mockFileClient = new();
        private readonly Mock<IAnalysisServiceClient> _mockAnalysisClient = new();
        private readonly ILogger<UploadController> _logger = Mock.Of<ILogger<UploadController>>();

        [Fact]
        public async Task UploadFile_ReturnsOk_ForValidTxtFile()
        {
            // Arrange
            var formFile = new FormFile(new MemoryStream(new byte[10]), 0, 10, "file", "test.txt")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            var fileResult = new FileUploadResult { FileId = "fileid" };
            var analysisResult = new AnalysisResult { AnalysisId = "analysisid" };

            _mockFileClient.Setup(c => c.UploadFileAsync(formFile)).ReturnsAsync(fileResult);
            _mockAnalysisClient.Setup(c => c.RequestAnalysisAsync(fileResult.FileId)).ReturnsAsync(analysisResult);

            var controller = new UploadController(_mockFileClient.Object, _mockAnalysisClient.Object, _logger);

            // Act
            var result = await controller.UploadFile(formFile);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<UploadResponse>(okResult.Value);
            Assert.Equal("fileid", value.FileId);
            Assert.Equal("analysisid", value.AnalysisId);
        }

        [Fact]
        public async Task UploadFile_ReturnsBadRequest_WhenNoFile()
        {
            // Arrange
            var controller = new UploadController(_mockFileClient.Object, _mockAnalysisClient.Object, _logger);

            // Act
            var result = await controller.UploadFile(null!);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}

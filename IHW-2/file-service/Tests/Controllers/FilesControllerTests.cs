using FileService.Controllers;
using FileService.Models;
using FileService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using Xunit;

namespace FileService.Tests.Controllers
{
    public class FilesControllerTests
    {
        private readonly Mock<IFileStorageService> _mockFileStorageService;
        private readonly Mock<ILogger<FilesController>> _mockLogger;
        private readonly FilesController _controller;

        public FilesControllerTests()
        {
            _mockFileStorageService = new Mock<IFileStorageService>();
            _mockLogger = new Mock<ILogger<FilesController>>();
            _controller = new FilesController(_mockFileStorageService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task UploadFile_ReturnsCreatedResultWhenFileUploaded()
        {
            // Arrange
            var fileContent = "This is a test file content.";
            var fileName = "test.txt";
            var fileId = Guid.NewGuid().ToString();
            
            var mockFile = CreateMockFile(fileContent, fileName, "text/plain");
            
            var uploadResult = new FileUploadResult
            {
                FileId = fileId,
                Filename = fileName,
                Size = fileContent.Length,
                CreatedAt = DateTime.UtcNow
            };
            
            _mockFileStorageService.Setup(s => s.SaveFileAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(uploadResult);

            // Act
            var result = await _controller.UploadFile(mockFile);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(FilesController.GetFileById), createdResult.ActionName);
            Assert.Equal(fileId, createdResult.RouteValues["id"]);
            
            var returnValue = Assert.IsType<FileUploadResult>(createdResult.Value);
            Assert.Equal(fileId, returnValue.FileId);
            Assert.Equal(fileName, returnValue.Filename);
        }

        [Fact]
        public async Task UploadFile_ReturnsBadRequestWhenFileIsNull()
        {
            // Act
            var result = await _controller.UploadFile(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            Assert.Equal("No file uploaded", errorResponse.Error);
        }

        [Fact]
        public async Task UploadFile_ReturnsBadRequestWhenFileTypeIsNotText()
        {
            // Arrange
            var fileContent = "This is a test file content.";
            var fileName = "test.jpg";
            var mockFile = CreateMockFile(fileContent, fileName, "image/jpeg");

            // Act
            var result = await _controller.UploadFile(mockFile);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            Assert.Equal("Only .txt files are allowed", errorResponse.Error);
        }

        [Fact]
        public async Task UploadFile_ReturnsBadRequestWhenArgumentExceptionThrown()
        {
            // Arrange
            var fileContent = "This is a test file content.";
            var fileName = "test.txt";
            var mockFile = CreateMockFile(fileContent, fileName, "text/plain");
            
            _mockFileStorageService.Setup(s => s.SaveFileAsync(It.IsAny<IFormFile>()))
                .ThrowsAsync(new ArgumentException("Invalid file"));

            // Act
            var result = await _controller.UploadFile(mockFile);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            Assert.Equal("Invalid file", errorResponse.Error);
        }

        [Fact]
        public async Task UploadFile_ReturnsServerErrorWhenExceptionThrown()
        {
            // Arrange
            var fileContent = "This is a test file content.";
            var fileName = "test.txt";
            var mockFile = CreateMockFile(fileContent, fileName, "text/plain");
            
            _mockFileStorageService.Setup(s => s.SaveFileAsync(It.IsAny<IFormFile>()))
                .ThrowsAsync(new Exception("Storage error"));

            // Act
            var result = await _controller.UploadFile(mockFile);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            var errorResponse = Assert.IsType<ErrorResponse>(statusCodeResult.Value);
            Assert.Equal("Error uploading file", errorResponse.Error);
        }

        [Fact]
        public async Task GetAllFiles_ReturnsOkWithFilesList()
        {
            // Arrange
            var files = new List<FileMetadataDto>
            {
                new FileMetadataDto { Id = "1", Filename = "file1.txt", Size = 100, CreatedAt = DateTime.UtcNow },
                new FileMetadataDto { Id = "2", Filename = "file2.txt", Size = 200, CreatedAt = DateTime.UtcNow }
            };
            
            _mockFileStorageService.Setup(s => s.GetAllFilesAsync())
                .ReturnsAsync(files);

            // Act
            var result = await _controller.GetAllFiles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<FileMetadataDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async Task GetAllFiles_ReturnsServerErrorWhenExceptionThrown()
        {
            // Arrange
            _mockFileStorageService.Setup(s => s.GetAllFilesAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetAllFiles();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            var errorResponse = Assert.IsType<ErrorResponse>(statusCodeResult.Value);
            Assert.Equal("Error retrieving files", errorResponse.Error);
        }

        [Fact]
        public async Task GetFileById_ReturnsOkWithFileData()
        {
            // Arrange
            var fileId = Guid.NewGuid().ToString();
            var file = new FileDto
            {
                Id = fileId,
                Filename = "test.txt",
                Content = "Test content",
                Size = 12,
                CreatedAt = DateTime.UtcNow
            };
            
            _mockFileStorageService.Setup(s => s.GetFileByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(file);

            // Act
            var result = await _controller.GetFileById(fileId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<FileDto>(okResult.Value);
            Assert.Equal(fileId, returnValue.Id);
            Assert.Equal(file.Content, returnValue.Content);
        }

        [Fact]
        public async Task GetFileById_ReturnsBadRequestWhenIdFormatIsInvalid()
        {
            // Act
            var result = await _controller.GetFileById("invalid-guid");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            Assert.Equal("Invalid file ID format", errorResponse.Error);
        }

        [Fact]
        public async Task GetFileById_ReturnsNotFoundWhenFileDoesNotExist()
        {
            // Arrange
            var fileId = Guid.NewGuid().ToString();
            
            _mockFileStorageService.Setup(s => s.GetFileByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((FileDto)null);

            // Act
            var result = await _controller.GetFileById(fileId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(notFoundResult.Value);
            Assert.Contains(fileId, errorResponse.Error);
        }

        private IFormFile CreateMockFile(string content, string fileName, string contentType)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);
            var mockFile = new Mock<IFormFile>();
            
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.Length).Returns(bytes.Length);
            mockFile.Setup(f => f.ContentType).Returns(contentType);
            mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
            
            return mockFile.Object;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiGateway.Controllers;
using ApiGateway.Models;
using ApiGateway.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ApiGateway.Tests.Controllers
{
    public class FilesControllerTests
    {
        private readonly Mock<IFileServiceClient> _mockFileServiceClient;
        private readonly Mock<ILogger<FilesController>> _mockLogger;
        private readonly FilesController _controller;

        public FilesControllerTests()
        {
            _mockFileServiceClient = new Mock<IFileServiceClient>();
            _mockLogger = new Mock<ILogger<FilesController>>();
            _controller = new FilesController(_mockFileServiceClient.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllFiles_ReturnsOkWithFilesList()
        {
            // Arrange
            var expectedFiles = new List<FileMetadata>
            {
                new FileMetadata { Id = "1", Filename = "file1.txt", Size = 100, CreatedAt = DateTime.UtcNow },
                new FileMetadata { Id = "2", Filename = "file2.txt", Size = 200, CreatedAt = DateTime.UtcNow }
            };

            _mockFileServiceClient.Setup(x => x.GetAllFilesAsync())
                .ReturnsAsync(expectedFiles);

            // Act
            var result = await _controller.GetAllFiles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<FileMetadata>>(okResult.Value);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async Task GetAllFiles_ReturnsInternalServerErrorWhenServiceFails()
        {
            // Arrange
            _mockFileServiceClient.Setup(x => x.GetAllFilesAsync())
                .ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _controller.GetAllFiles();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var errorResponse = Assert.IsType<ErrorResponse>(statusCodeResult.Value);
            Assert.Equal("Error retrieving files", errorResponse.Error);
        }

        [Fact]
        public async Task GetFileById_ReturnsOkWithFileData()
        {
            // Arrange
            var fileId = "123";
            var expectedFile = new FileDto
            {
                Id = fileId,
                Filename = "file1.txt",
                Content = "Test content",
                Size = 100,
                CreatedAt = DateTime.UtcNow
            };

            _mockFileServiceClient.Setup(x => x.GetFileByIdAsync(fileId))
                .ReturnsAsync(expectedFile);

            // Act
            var result = await _controller.GetFileById(fileId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<FileDto>(okResult.Value);
            Assert.Equal(fileId, returnValue.Id);
            Assert.Equal(expectedFile.Content, returnValue.Content);
        }

        [Fact]
        public async Task GetFileById_ReturnsNotFoundWhenFileDoesNotExist()
        {
            // Arrange
            var fileId = "nonexistent";

            _mockFileServiceClient.Setup(x => x.GetFileByIdAsync(fileId))
                .ThrowsAsync(new KeyNotFoundException($"File with ID {fileId} not found"));

            // Act
            var result = await _controller.GetFileById(fileId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(notFoundResult.Value);
            Assert.Contains(fileId, errorResponse.Error);
        }

        [Fact]
        public async Task GetFileById_ReturnsInternalServerErrorWhenServiceFails()
        {
            // Arrange
            var fileId = "123";

            _mockFileServiceClient.Setup(x => x.GetFileByIdAsync(fileId))
                .ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _controller.GetFileById(fileId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var errorResponse = Assert.IsType<ErrorResponse>(statusCodeResult.Value);
            Assert.Contains(fileId, errorResponse.Error);
        }
    }
}

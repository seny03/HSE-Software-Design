using FileService.Data;
using FileService.Models;
using FileService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Moq;
using System.Text;
using Xunit;

namespace FileService.Tests.Services
{
    public class FileStorageServiceTests
    {
        private readonly DbContextOptions<FileDbContext> _dbContextOptions;
        private readonly Mock<ILogger<FileStorageService>> _mockLogger;
        private readonly Mock<IWebHostEnvironment> _mockEnvironment;
        private readonly string _testUploadsDirectory;

        public FileStorageServiceTests()
        {
            // Set up in-memory database
            _dbContextOptions = new DbContextOptionsBuilder<FileDbContext>()
                .UseInMemoryDatabase(databaseName: $"FileDb_{Guid.NewGuid()}")
                .Options;

            _mockLogger = new Mock<ILogger<FileStorageService>>();
            _mockEnvironment = new Mock<IWebHostEnvironment>();
            
            // Set up test uploads directory
            _testUploadsDirectory = Path.Combine(Path.GetTempPath(), "FileServiceTests", "uploads");
            _mockEnvironment.Setup(e => e.ContentRootPath).Returns(Path.GetTempPath());
            
            Directory.CreateDirectory(_testUploadsDirectory);
        }

        [Fact]
        public async Task SaveFileAsync_SavesFileAndReturnsResult()
        {
            // Arrange
            using var context = new FileDbContext(_dbContextOptions);
            var service = new FileStorageService(context, _mockLogger.Object, _mockEnvironment.Object);

            var fileContent = "This is a test file content.";
            var fileName = "test.txt";
            var mockFile = CreateMockFile(fileContent, fileName, "text/plain");

            // Act
            var result = await service.SaveFileAsync(mockFile);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(fileName, result.Filename);
            Assert.Equal(fileContent.Length, result.Size);

            // Verify database
            var savedFile = await context.Files.FirstOrDefaultAsync();
            Assert.NotNull(savedFile);
            Assert.Equal(fileName, savedFile.Filename);
            Assert.Equal(fileContent, savedFile.Content);
        }

        [Fact]
        public async Task SaveFileAsync_ThrowsExceptionForNonTextFile()
        {
            // Arrange
            using var context = new FileDbContext(_dbContextOptions);
            var service = new FileStorageService(context, _mockLogger.Object, _mockEnvironment.Object);

            var fileContent = "This is a test file content.";
            var fileName = "test.jpg";
            var mockFile = CreateMockFile(fileContent, fileName, "image/jpeg");

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.SaveFileAsync(mockFile));
        }

        [Fact]
        public async Task GetAllFilesAsync_ReturnsAllFiles()
        {
            // Arrange
            using var context = new FileDbContext(_dbContextOptions);
            
            // Add test files to the database
            context.Files.Add(new FileEntity
            {
                Id = Guid.NewGuid(),
                Filename = "file1.txt",
                FilePath = "/path/to/file1",
                Size = 100,
                Content = "Content 1",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            });
            
            context.Files.Add(new FileEntity
            {
                Id = Guid.NewGuid(),
                Filename = "file2.txt",
                FilePath = "/path/to/file2",
                Size = 200,
                Content = "Content 2",
                CreatedAt = DateTime.UtcNow
            });
            
            await context.SaveChangesAsync();
            
            var service = new FileStorageService(context, _mockLogger.Object, _mockEnvironment.Object);

            // Act
            var result = await service.GetAllFilesAsync();

            // Assert
            var files = result.ToList();
            Assert.Equal(2, files.Count);
            Assert.Contains(files, f => f.Filename == "file1.txt");
            Assert.Contains(files, f => f.Filename == "file2.txt");
        }

        [Fact]
        public async Task GetFileByIdAsync_ReturnsFileWhenExists()
        {
            // Arrange
            using var context = new FileDbContext(_dbContextOptions);
            
            var fileId = Guid.NewGuid();
            var fileName = "test.txt";
            var fileContent = "Test content";
            
            context.Files.Add(new FileEntity
            {
                Id = fileId,
                Filename = fileName,
                FilePath = "/path/to/file",
                Size = fileContent.Length,
                Content = fileContent,
                CreatedAt = DateTime.UtcNow
            });
            
            await context.SaveChangesAsync();
            
            var service = new FileStorageService(context, _mockLogger.Object, _mockEnvironment.Object);

            // Act
            var result = await service.GetFileByIdAsync(fileId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(fileId.ToString(), result.Id);
            Assert.Equal(fileName, result.Filename);
            Assert.Equal(fileContent, result.Content);
        }

        [Fact]
        public async Task GetFileByIdAsync_ReturnsNullWhenFileDoesNotExist()
        {
            // Arrange
            using var context = new FileDbContext(_dbContextOptions);
            var service = new FileStorageService(context, _mockLogger.Object, _mockEnvironment.Object);

            // Act
            var result = await service.GetFileByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteFileAsync_DeletesFileWhenExists()
        {
            // Arrange
            using var context = new FileDbContext(_dbContextOptions);
            
            var fileId = Guid.NewGuid();
            var fileName = "test.txt";
            var filePath = Path.Combine(_testUploadsDirectory, fileId.ToString());
            
            // Create test file on disk
            await File.WriteAllTextAsync(filePath, "Test content");
            
            context.Files.Add(new FileEntity
            {
                Id = fileId,
                Filename = fileName,
                FilePath = filePath,
                Size = 100,
                Content = "Test content",
                CreatedAt = DateTime.UtcNow
            });
            
            await context.SaveChangesAsync();
            
            var service = new FileStorageService(context, _mockLogger.Object, _mockEnvironment.Object);

            // Act
            var result = await service.DeleteFileAsync(fileId);

            // Assert
            Assert.True(result);
            Assert.False(File.Exists(filePath)); // File should be deleted from disk
            Assert.Empty(await context.Files.ToListAsync()); // File should be deleted from DB
        }

        [Fact]
        public async Task DeleteFileAsync_ReturnsFalseWhenFileDoesNotExist()
        {
            // Arrange
            using var context = new FileDbContext(_dbContextOptions);
            var service = new FileStorageService(context, _mockLogger.Object, _mockEnvironment.Object);

            // Act
            var result = await service.DeleteFileAsync(Guid.NewGuid());

            // Assert
            Assert.False(result);
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

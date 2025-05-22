using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AnalysisService.Models;
using AnalysisService.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace AnalysisService.Tests.Services
{
    public class FileClientServiceTests
    {
        [Fact]
        public async Task GetFileByIdAsync_ReturnsFile_WhenSuccess()
        {
            // Arrange
            var fileId = "1";
            var fileDto = new FileDto
            {
                Id = fileId,
                Filename = "test.txt",
                Content = "sample content",
                Size = 100,
                CreatedAt = DateTime.UtcNow
            };

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(fileDto), Encoding.UTF8, "application/json")
            };

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get && r.RequestUri!.ToString().Contains($"/files/{fileId}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response)
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost/")
            };

            var loggerMock = new Mock<ILogger<FileClientService>>();
            var clientFactory = new Mock<IHttpClientFactory>();
            clientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var fileClient = new FileClientService(clientFactory.Object, loggerMock.Object);

            // Act
            var result = await fileClient.GetFileByIdAsync(fileId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(fileDto.Filename, result.Filename);
            Assert.Equal(fileDto.Content, result.Content);
        }

        [Fact]
        public async Task GetFileByIdAsync_ThrowsKeyNotFound_WhenNotFound()
        {
            // Arrange
            var fileId = "unknown";
            var response = new HttpResponseMessage(HttpStatusCode.NotFound);

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get && r.RequestUri!.ToString().Contains($"/files/{fileId}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response)
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost/")
            };

            var loggerMock = new Mock<ILogger<FileClientService>>();
            var clientFactory = new Mock<IHttpClientFactory>();
            clientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var fileClient = new FileClientService(clientFactory.Object, loggerMock.Object);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => fileClient.GetFileByIdAsync(fileId));
            Assert.Contains(fileId, ex.Message);
        }

        [Fact]
        public async Task GetFileContentsAsync_ReturnsDictionary_WhenAllFilesFound()
        {
            // Arrange
            var fileIds = new List<string> { "1", "2" };
            var fileDtos = new Dictionary<string, FileDto>
            {
                { "1", new FileDto { Id = "1", Filename = "1.txt", Content = "A", Size = 1, CreatedAt = DateTime.UtcNow } },
                { "2", new FileDto { Id = "2", Filename = "2.txt", Content = "B", Size = 1, CreatedAt = DateTime.UtcNow } }
            };

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            foreach (var id in fileIds)
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(fileDtos[id]), Encoding.UTF8, "application/json")
                };

                handlerMock.Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get && r.RequestUri!.ToString().Contains($"/files/{id}")),
                        ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(response)
                    .Verifiable();
            }

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost/")
            };
            var loggerMock = new Mock<ILogger<FileClientService>>();
            var clientFactory = new Mock<IHttpClientFactory>();
            clientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var fileClient = new FileClientService(clientFactory.Object, loggerMock.Object);

            // Act
            var result = await fileClient.GetFileContentsAsync(fileIds);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("A", result["1"]);
            Assert.Equal("B", result["2"]);
        }

        [Fact]
        public async Task GetFileContentsAsync_ThrowsException_IfOneFileNotFound()
        {
            // Arrange
            var fileIds = new List<string> { "1", "404" };

            var file1 = new FileDto { Id = "1", Filename = "1.txt", Content = "A", Size = 1, CreatedAt = DateTime.UtcNow };
            var responseOk = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(file1), Encoding.UTF8, "application/json")
            };
            var response404 = new HttpResponseMessage(HttpStatusCode.NotFound);

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get && r.RequestUri!.ToString().Contains("/files/1")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseOk)
                .Verifiable();

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get && r.RequestUri!.ToString().Contains("/files/404")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response404)
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost/")
            };
            var loggerMock = new Mock<ILogger<FileClientService>>();
            var clientFactory = new Mock<IHttpClientFactory>();
            clientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var fileClient = new FileClientService(clientFactory.Object, loggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => fileClient.GetFileContentsAsync(fileIds));
        }
    }
}

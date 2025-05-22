using ApiGateway.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ApiGateway.Tests.Services
{
    public class FileServiceClientTests
    {
        [Fact]
        public void Can_Construct_FileServiceClient()
        {
            // Arrange
            var mockFactory = new Mock<IHttpClientFactory>();
            var mockLogger = new Mock<ILogger<FileServiceClient>>();

            mockFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(new System.Net.Http.HttpClient());

            // Act
            var client = new FileServiceClient(mockFactory.Object, mockLogger.Object);

            // Assert
            Assert.NotNull(client);
        }
    }
}

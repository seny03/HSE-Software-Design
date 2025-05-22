using ApiGateway.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ApiGateway.Tests.Services
{
    public class AnalysisServiceClientTests
    {
        [Fact]
        public void Can_Construct_AnalysisServiceClient()
        {
            // Arrange
            var mockFactory = new Mock<IHttpClientFactory>();
            var mockLogger = new Mock<ILogger<AnalysisServiceClient>>();

            // Подсунем заглушку клиента
            mockFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(new System.Net.Http.HttpClient());

            // Act
            var client = new AnalysisServiceClient(mockFactory.Object, mockLogger.Object);

            // Assert
            Assert.NotNull(client);
        }
    }
}

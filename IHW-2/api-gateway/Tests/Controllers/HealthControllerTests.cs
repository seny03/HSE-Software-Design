using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class HealthControllerTests
    {
        private readonly Mock<IFileServiceClient> _mockFileServiceClient;
        private readonly Mock<IAnalysisServiceClient> _mockAnalysisServiceClient;
        private readonly Mock<ILogger<HealthController>> _mockLogger;
        private readonly HealthController _controller;

        public HealthControllerTests()
        {
            _mockFileServiceClient = new Mock<IFileServiceClient>();
            _mockAnalysisServiceClient = new Mock<IAnalysisServiceClient>();
            _mockLogger = new Mock<ILogger<HealthController>>();
            _controller = new HealthController(_mockFileServiceClient.Object, _mockAnalysisServiceClient.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetHealthStatus_ReturnsOkWithServiceStatuses()
        {
            // Arrange
            var fileServiceHealth = new ServiceHealthStatus { Status = "up" };
            var analysisServiceHealth = new ServiceHealthStatus { Status = "up" };

            _mockFileServiceClient.Setup(x => x.CheckHealthAsync())
                .ReturnsAsync(fileServiceHealth);

            _mockAnalysisServiceClient.Setup(x => x.CheckHealthAsync())
                .ReturnsAsync(analysisServiceHealth);

            // Act
            var result = await _controller.GetHealthStatus();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var statusResult = Assert.IsType<GatewayHealthStatus>(okResult.Value);
            Assert.Equal("up", statusResult.Status);
            Assert.Equal("up", statusResult.Services["fileService"]);
            Assert.Equal("up", statusResult.Services["analysisService"]);
        }

        [Fact]
        public async Task GetHealthStatus_HandlesFileServiceDown()
        {
            // Arrange
            var fileServiceHealth = new ServiceHealthStatus { Status = "down" };
            var analysisServiceHealth = new ServiceHealthStatus { Status = "up" };

            _mockFileServiceClient.Setup(x => x.CheckHealthAsync())
                .ReturnsAsync(fileServiceHealth);

            _mockAnalysisServiceClient.Setup(x => x.CheckHealthAsync())
                .ReturnsAsync(analysisServiceHealth);

            // Act
            var result = await _controller.GetHealthStatus();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var statusResult = Assert.IsType<GatewayHealthStatus>(okResult.Value);
            Assert.Equal("up", statusResult.Status);
            Assert.Equal("down", statusResult.Services["fileService"]);
            Assert.Equal("up", statusResult.Services["analysisService"]);
        }

        [Fact]
        public async Task GetHealthStatus_HandlesAnalysisServiceDown()
        {
            // Arrange
            var fileServiceHealth = new ServiceHealthStatus { Status = "up" };
            var analysisServiceHealth = new ServiceHealthStatus { Status = "down" };

            _mockFileServiceClient.Setup(x => x.CheckHealthAsync())
                .ReturnsAsync(fileServiceHealth);

            _mockAnalysisServiceClient.Setup(x => x.CheckHealthAsync())
                .ReturnsAsync(analysisServiceHealth);

            // Act
            var result = await _controller.GetHealthStatus();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var statusResult = Assert.IsType<GatewayHealthStatus>(okResult.Value);
            Assert.Equal("up", statusResult.Status);
            Assert.Equal("up", statusResult.Services["fileService"]);
            Assert.Equal("down", statusResult.Services["analysisService"]);
        }

        [Fact]
        public async Task GetHealthStatus_HandlesBothServicesDown()
        {
            // Arrange
            var fileServiceHealth = new ServiceHealthStatus { Status = "down" };
            var analysisServiceHealth = new ServiceHealthStatus { Status = "down" };

            _mockFileServiceClient.Setup(x => x.CheckHealthAsync())
                .ReturnsAsync(fileServiceHealth);

            _mockAnalysisServiceClient.Setup(x => x.CheckHealthAsync())
                .ReturnsAsync(analysisServiceHealth);

            // Act
            var result = await _controller.GetHealthStatus();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var statusResult = Assert.IsType<GatewayHealthStatus>(okResult.Value);
            Assert.Equal("up", statusResult.Status);
            Assert.Equal("down", statusResult.Services["fileService"]);
            Assert.Equal("down", statusResult.Services["analysisService"]);
        }

        [Fact]
        public async Task GetHealthStatus_HandlesExceptionsInFileServiceCheck()
        {
            // Arrange
            var analysisServiceHealth = new ServiceHealthStatus { Status = "up" };

            _mockFileServiceClient.Setup(x => x.CheckHealthAsync())
                .ThrowsAsync(new Exception("Connection error"));

            _mockAnalysisServiceClient.Setup(x => x.CheckHealthAsync())
                .ReturnsAsync(analysisServiceHealth);

            // Act
            var result = await _controller.GetHealthStatus();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var statusResult = Assert.IsType<GatewayHealthStatus>(okResult.Value);
            Assert.Equal("up", statusResult.Status);
            Assert.Contains("fileService", statusResult.Services.Keys);
            Assert.Equal("up", statusResult.Services["analysisService"]);
        }
    }
}

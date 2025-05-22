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

namespace ApiGateway.Tests
{
    public class HealthControllerTests
    {
        [Fact]
        public void HealthController_ReturnsOk()
        {
            // Arrange
            var mockFileClient = new Mock<IFileServiceClient>();
            var mockAnalysisClient = new Mock<IAnalysisServiceClient>();
            var mockLogger = new Mock<ILogger<HealthController>>();
            
            mockFileClient.Setup(x => x.CheckHealthAsync())
                .ReturnsAsync(new ServiceHealthStatus { Status = "up" });
                
            mockAnalysisClient.Setup(x => x.CheckHealthAsync())
                .ReturnsAsync(new ServiceHealthStatus { Status = "up" });
                
            var controller = new HealthController(
                mockFileClient.Object, 
                mockAnalysisClient.Object, 
                mockLogger.Object);

            // Act
            var result = controller.GetHealthStatus().Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var statusResult = Assert.IsType<GatewayHealthStatus>(okResult.Value);
            Assert.Equal("up", statusResult.Status);
        }
    }
    
    public class FilesControllerTests
    {
        [Fact]
        public void FilesController_ReturnsOk()
        {
            // Arrange
            var mockFileClient = new Mock<IFileServiceClient>();
            var mockLogger = new Mock<ILogger<FilesController>>();
            
            mockFileClient.Setup(x => x.GetAllFilesAsync())
                .ReturnsAsync(new List<FileMetadata>());
                
            var controller = new FilesController(mockFileClient.Object, mockLogger.Object);

            // Act
            var result = controller.GetAllFiles().Result;

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
    }
}

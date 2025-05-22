using System.Collections.Generic;
using System.Threading.Tasks;
using AnalysisService.Controllers;
using AnalysisService.Models;
using AnalysisService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AnalysisService.Tests.Controllers
{
    public class CompareControllerTests
    {
        [Fact]
        public async Task CompareFiles_ReturnsComparisonResults()
        {
            // Arrange
            var mockService = new Mock<ITextAnalysisService>();
            var mockFileClient = new Mock<IFileClientService>();
            var mockLogger = new Mock<ILogger<CompareController>>();
            var fileIds = new List<string> { "file1", "file2" };

            var request = new ComparisonRequest { FileIds = fileIds };
            var expectedResult = new List<ComparisonResult>
            {
                new ComparisonResult
                {
                    ComparisonId = "cmp1",
                    FileId1 = "file1",
                    FileId2 = "file2",
                    SimilarityScore = 0.9,
                    IsPlagiarism = true
                }
            };

            // Ожидаем, что сервис вызовет CompareTextsAsync с любыми аргументами и вернет expectedResult
            mockService.Setup(s => s.CompareTextsAsync(It.IsAny<List<string>>(), It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync(expectedResult);

            // Создаем контроллер со всеми нужными зависимостями
            var controller = new CompareController(
                mockService.Object,
                mockFileClient.Object,
                mockLogger.Object
            );

            // Act
            var result = await controller.CompareFiles(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actual = Assert.IsType<List<ComparisonResult>>(okResult.Value);
            Assert.Single(actual);
            Assert.Equal(0.9, actual[0].SimilarityScore, 2);
        }
    }
}

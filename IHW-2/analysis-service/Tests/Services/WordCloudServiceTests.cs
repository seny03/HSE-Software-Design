using AnalysisService.Data;
using AnalysisService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AnalysisService.Tests.Services
{
    public class WordCloudServiceTests
    {
        [Fact]
        public void GenerateWordCloudUrl_ReturnsNonEmptyUrl()
        {
            // Создаём опции для in-memory DbContext
            var options = new DbContextOptionsBuilder<AnalysisDbContext>()
                .UseInMemoryDatabase("TestDb_WordCloud")
                .Options;

            var dbContext = new AnalysisDbContext(options);
            var mockLogger = new Mock<ILogger<WordCloudService>>();
            var service = new WordCloudService(dbContext, mockLogger.Object);

            var text = "test test test word word cloud";
            var url = service.GenerateWordCloudUrl(text);

            Assert.False(string.IsNullOrWhiteSpace(url));
            Assert.Contains("quickchart.io/wordcloud", url);
        }
    }
}

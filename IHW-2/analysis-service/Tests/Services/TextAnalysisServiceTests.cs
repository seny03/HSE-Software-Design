using AnalysisService.Data;
using AnalysisService.Models;
using AnalysisService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AnalysisService.Tests.Services
{
    public class TextAnalysisServiceTests
    {
        private readonly DbContextOptions<AnalysisDbContext> _dbContextOptions;
        private readonly Mock<IWordCloudService> _mockWordCloudService;
        private readonly Mock<ILogger<TextAnalysisService>> _mockLogger;

        public TextAnalysisServiceTests()
        {
            // Set up in-memory database
            _dbContextOptions = new DbContextOptionsBuilder<AnalysisDbContext>()
                .UseInMemoryDatabase(databaseName: $"AnalysisDb_{Guid.NewGuid()}")
                .Options;

            _mockWordCloudService = new Mock<IWordCloudService>();
            _mockLogger = new Mock<ILogger<TextAnalysisService>>();
        }

        [Fact]
        public async Task AnalyzeTextAsync_ReturnsNewAnalysis_WhenFileNotAnalyzedBefore()
        {
            // Arrange
            using var context = new AnalysisDbContext(_dbContextOptions);
            var service = new TextAnalysisService(context, _mockWordCloudService.Object, _mockLogger.Object);
            
            var fileId = Guid.NewGuid().ToString();
            var fileContent = "This is a test content.\n\nThis is a second paragraph.";
            var wordCloudUrl = "https://example.com/wordcloud/123";
            
            _mockWordCloudService.Setup(x => x.GetOrGenerateWordCloudUrlAsync(fileId, fileContent))
                .ReturnsAsync(wordCloudUrl);

            // Act
            var result = await service.AnalyzeTextAsync(fileId, fileContent);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(fileId, result.FileId);
            Assert.Equal(2, result.ParagraphCount);
            Assert.Equal(10, result.WordCount);
            Assert.Equal(wordCloudUrl, result.WordCloudUrl);
            
            // Verify saved to database
            var analysis = await context.Analyses.FirstOrDefaultAsync(a => a.FileId == Guid.Parse(fileId));
            Assert.NotNull(analysis);
            Assert.Equal(2, analysis.ParagraphCount);
            Assert.Equal(10, analysis.WordCount);
        }

        [Fact]
        public async Task AnalyzeTextAsync_ReturnsExistingAnalysis_WhenFileAlreadyAnalyzed()
        {
            // Arrange
            using var context = new AnalysisDbContext(_dbContextOptions);
            
            var fileId = Guid.NewGuid();
            var existingAnalysis = new AnalysisEntity
            {
                Id = Guid.NewGuid(),
                FileId = fileId,
                ParagraphCount = 3,
                WordCount = 15,
                CharacterCount = 75,
                WordCloudUrl = "https://example.com/wordcloud/existing",
                CreatedAt = DateTime.UtcNow.AddHours(-1)
            };
            
            context.Analyses.Add(existingAnalysis);
            await context.SaveChangesAsync();
            
            var service = new TextAnalysisService(context, _mockWordCloudService.Object, _mockLogger.Object);

            // Act
            var result = await service.AnalyzeTextAsync(fileId.ToString(), "New content that should be ignored");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(fileId.ToString(), result.FileId);
            Assert.Equal(existingAnalysis.Id.ToString(), result.AnalysisId);
            Assert.Equal(existingAnalysis.ParagraphCount, result.ParagraphCount);
            Assert.Equal(existingAnalysis.WordCount, result.WordCount);
            Assert.Equal(existingAnalysis.CharacterCount, result.CharacterCount);
            Assert.Equal(existingAnalysis.WordCloudUrl, result.WordCloudUrl);
            
            // Verify no new entity was added
            Assert.Equal(1, await context.Analyses.CountAsync());
        }

        [Fact]
        public async Task GetAnalysisAsync_ReturnsAnalysis_WhenExists()
        {
            // Arrange
            using var context = new AnalysisDbContext(_dbContextOptions);
            
            var analysisId = Guid.NewGuid();
            var fileId = Guid.NewGuid();
            var analysis = new AnalysisEntity
            {
                Id = analysisId,
                FileId = fileId,
                ParagraphCount = 2,
                WordCount = 10,
                CharacterCount = 50,
                WordCloudUrl = "https://example.com/wordcloud/123",
                CreatedAt = DateTime.UtcNow.AddHours(-1)
            };
            
            context.Analyses.Add(analysis);
            await context.SaveChangesAsync();
            
            var service = new TextAnalysisService(context, _mockWordCloudService.Object, _mockLogger.Object);

            // Act
            var result = await service.GetAnalysisAsync(analysisId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(analysisId.ToString(), result.AnalysisId);
            Assert.Equal(fileId.ToString(), result.FileId);
            Assert.Equal(analysis.ParagraphCount, result.ParagraphCount);
            Assert.Equal(analysis.WordCount, result.WordCount);
            Assert.Equal(analysis.CharacterCount, result.CharacterCount);
        }

        [Fact]
        public async Task GetAnalysisAsync_ReturnsNull_WhenNotExists()
        {
            // Arrange
            using var context = new AnalysisDbContext(_dbContextOptions);
            var service = new TextAnalysisService(context, _mockWordCloudService.Object, _mockLogger.Object);

            // Act
            var result = await service.GetAnalysisAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CompareTextsAsync_ReturnsComparisonResults()
        {
            // Arrange
            using var context = new AnalysisDbContext(_dbContextOptions);
            var service = new TextAnalysisService(context, _mockWordCloudService.Object, _mockLogger.Object);
            
            var fileId1 = Guid.NewGuid().ToString();
            var fileId2 = Guid.NewGuid().ToString();
            var fileId3 = Guid.NewGuid().ToString();
            
            var fileContents = new Dictionary<string, string>
            {
                { fileId1, "This is a test content for file 1." },
                { fileId2, "This is a very similar test content for file 2." },
                { fileId3, "This is completely different content." }
            };
            
            var fileIds = new List<string> { fileId1, fileId2, fileId3 };

            // Act
            var results = await service.CompareTextsAsync(fileIds, fileContents);

            // Assert
            Assert.Equal(3, results.Count); // 3 comparisons for 3 files (1-2, 1-3, 2-3)
            
            // Check each comparison is stored in database
            Assert.Equal(3, await context.Comparisons.CountAsync());
            
            // Check each result has expected properties
            foreach (var result in results)
            {
                Assert.NotEmpty(result.ComparisonId);
                Assert.Contains(result.FileId1, fileIds);
                Assert.Contains(result.FileId2, fileIds);
                Assert.True(result.SimilarityScore >= 0 && result.SimilarityScore <= 1);
                // IsPlagiarism is set to true if similarity > 0.8
                Assert.Equal(result.SimilarityScore > 0.8, result.IsPlagiarism);
            }
            
            // Check that file 1 and 2 have higher similarity than file 1 and 3
            var comparison1_2 = results.First(r => 
                (r.FileId1 == fileId1 && r.FileId2 == fileId2) || 
                (r.FileId1 == fileId2 && r.FileId2 == fileId1));
                
            var comparison1_3 = results.First(r => 
                (r.FileId1 == fileId1 && r.FileId2 == fileId3) || 
                (r.FileId1 == fileId3 && r.FileId2 == fileId1));
                
            Assert.True(comparison1_2.SimilarityScore > comparison1_3.SimilarityScore);
        }

        [Fact]
        public void CalculateSimilarity_ReturnsValidSimilarityScore()
        {
            // Arrange
            using var context = new AnalysisDbContext(_dbContextOptions);
            var service = new TextAnalysisService(context, _mockWordCloudService.Object, _mockLogger.Object);
            
            var text1 = "This is a test.";
            var text2 = "This is a test.";
            var text3 = "This is also a test.";
            var text4 = "Completely different text.";

            // Act
            var similarity1_2 = service.CalculateSimilarity(text1, text2);
            var similarity1_3 = service.CalculateSimilarity(text1, text3);
            var similarity1_4 = service.CalculateSimilarity(text1, text4);

            // Assert
            Assert.Equal(1.0, similarity1_2); // Identical texts should have similarity 1.0
            Assert.True(similarity1_3 < 1.0 && similarity1_3 > similarity1_4); // Similar texts should have higher similarity than different texts
            Assert.True(similarity1_4 < similarity1_3); // Different texts should have lower similarity
        }

        [Fact]
        public void CountStatistics_ReturnsAccurateStatistics()
        {
            // Arrange
            using var context = new AnalysisDbContext(_dbContextOptions);
            var service = new TextAnalysisService(context, _mockWordCloudService.Object, _mockLogger.Object);
            
            var text = "This is the first paragraph.\n\nThis is the second paragraph with some more words.\n\nThird paragraph.";

            // Act
            var stats = service.CountStatistics(text);

            // Assert
            Assert.Equal(3, stats.ParagraphCount);
            Assert.Equal(16, stats.WordCount);
            Assert.Equal(81, stats.CharacterCount);
        }

        [Fact]
        public void CountStatistics_HandlesEmptyText()
        {
            // Arrange
            using var context = new AnalysisDbContext(_dbContextOptions);
            var service = new TextAnalysisService(context, _mockWordCloudService.Object, _mockLogger.Object);

            // Act
            var stats = service.CountStatistics(string.Empty);

            // Assert
            Assert.Equal(0, stats.ParagraphCount);
            Assert.Equal(0, stats.WordCount);
            Assert.Equal(0, stats.CharacterCount);
        }
    }
}

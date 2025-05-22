using ApiGateway.Models;
using System;
using Xunit;

namespace ApiGateway.Tests.Models
{
    public class AnalysisResultTests
    {
        [Fact]
        public void Can_Create_AnalysisResult()
        {
            var now = DateTime.UtcNow;
            var result = new AnalysisResult
            {
                AnalysisId = "A1",
                FileId = "F1",
                ParagraphCount = 2,
                WordCount = 5,
                CharacterCount = 10,
                WordCloudUrl = "url",
                CreatedAt = now
            };
            Assert.Equal("A1", result.AnalysisId);
            Assert.Equal("F1", result.FileId);
            Assert.Equal(2, result.ParagraphCount);
            Assert.Equal(5, result.WordCount);
            Assert.Equal(10, result.CharacterCount);
            Assert.Equal("url", result.WordCloudUrl);
            Assert.Equal(now, result.CreatedAt);
        }
    }
}

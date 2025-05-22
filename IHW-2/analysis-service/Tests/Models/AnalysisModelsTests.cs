using AnalysisService.Models;
using System;
using Xunit;

namespace AnalysisService.Tests.Models
{
    public class AnalysisModelsTests
    {
        [Fact]
        public void AnalysisResult_CanBeCreated()
        {
            var now = DateTime.UtcNow;
            var model = new AnalysisResult
            {
                AnalysisId = "id1",
                FileId = "id2",
                ParagraphCount = 2,
                WordCount = 5,
                CharacterCount = 22,
                WordCloudUrl = "http://test.com",
                CreatedAt = now
            };

            Assert.Equal("id1", model.AnalysisId);
            Assert.Equal("id2", model.FileId);
            Assert.Equal(2, model.ParagraphCount);
            Assert.Equal(5, model.WordCount);
            Assert.Equal(22, model.CharacterCount);
            Assert.Equal("http://test.com", model.WordCloudUrl);
            Assert.Equal(now, model.CreatedAt);
        }

        [Fact]
        public void ErrorResponse_CanBeCreated()
        {
            var model = new ErrorResponse { Error = "test" };
            Assert.Equal("test", model.Error);
        }
    }
}

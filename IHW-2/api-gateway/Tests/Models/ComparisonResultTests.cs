using ApiGateway.Models;
using Xunit;

namespace ApiGateway.Tests.Models
{
    public class ComparisonResultTests
    {
        [Fact]
        public void Can_Create_ComparisonResult()
        {
            var result = new ComparisonResult
            {
                ComparisonId = "C1",
                FileId1 = "F1",
                FileId2 = "F2",
                SimilarityScore = 0.77,
                IsPlagiarism = true
            };
            Assert.Equal("C1", result.ComparisonId);
            Assert.Equal("F1", result.FileId1);
            Assert.Equal("F2", result.FileId2);
            Assert.Equal(0.77, result.SimilarityScore);
            Assert.True(result.IsPlagiarism);
        }
    }
}

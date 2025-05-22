using ApiGateway.Models;
using Xunit;

namespace ApiGateway.Tests.Models
{
    public class AnalysisRequestTests
    {
        [Fact]
        public void Can_Create_AnalysisRequest()
        {
            var request = new AnalysisRequest { FileId = "file-123" };
            Assert.Equal("file-123", request.FileId);
        }
    }
}

using System.Collections.Generic;
using AnalysisService.Models;
using Xunit;

namespace AnalysisService.Tests.Models
{
    public class ComparisonRequestTests
    {
        [Fact]
        public void Can_Set_And_Get_FileIds()
        {
            var request = new ComparisonRequest();
            var ids = new List<string> { "id1", "id2" };
            request.FileIds = ids;

            Assert.Equal(2, request.FileIds.Count);
            Assert.Contains("id1", request.FileIds);
            Assert.Contains("id2", request.FileIds);
        }
    }
}

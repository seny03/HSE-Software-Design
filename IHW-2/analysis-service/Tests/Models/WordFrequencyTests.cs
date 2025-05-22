using AnalysisService.Models;
using Xunit;

namespace AnalysisService.Tests.Models
{
    public class WordFrequencyTests
    {
        [Fact]
        public void Can_Set_And_Get_Properties()
        {
            var freq = new WordFrequency
            {
                Word = "hello",
                Value = 7
            };

            Assert.Equal("hello", freq.Word);
            Assert.Equal(7, freq.Value);
        }
    }
}

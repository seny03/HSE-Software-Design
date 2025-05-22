using ApiGateway.Models;
using System;
using Xunit;

namespace ApiGateway.Tests.Models
{
    public class FileUploadResultTests
    {
        [Fact]
        public void Can_Create_FileUploadResult()
        {
            var now = DateTime.UtcNow;
            var result = new FileUploadResult
            {
                FileId = "file",
                Filename = "file.txt",
                Size = 123,
                CreatedAt = now
            };
            Assert.Equal("file", result.FileId);
            Assert.Equal("file.txt", result.Filename);
            Assert.Equal(123, result.Size);
            Assert.Equal(now, result.CreatedAt);
        }
    }
}

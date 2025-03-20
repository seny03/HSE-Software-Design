using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FinancialAccounting.DataImportExport.DataImport;

namespace FinancialAccounting.Tests.Commands
{
    public class ImportDataCommandTests
    {
        [Fact]
        public async Task ExecuteAsync_CallsImporterImport()
        {
            
            var mockImporter = new Mock<ImporterBase>();
            string filePath = "test_import.json";

            mockImporter.Setup(i => i.Import(filePath))
                .Returns(Task.CompletedTask);

            var command = new ImportDataCommand(mockImporter.Object, filePath);

            
            await command.ExecuteAsync();

            
            mockImporter.Verify(i => i.Import(filePath), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_WithNullImporter_ThrowsArgumentNullException()
        {
            
            ImporterBase nullImporter = null;
            string filePath = "test_import.json";

            
            Assert.Throws<ArgumentNullException>(() => new ImportDataCommand(nullImporter, filePath));
        }

        [Fact]
        public async Task ExecuteAsync_WithNullFilePath_ThrowsArgumentNullException()
        {
            
            var mockImporter = new Mock<ImporterBase>();
            string nullFilePath = null;

            
            Assert.Throws<ArgumentNullException>(() => new ImportDataCommand(mockImporter.Object, nullFilePath));
        }

        [Fact]
        public async Task ExecuteAsync_WithEmptyFilePath_ThrowsArgumentException()
        {
            
            var mockImporter = new Mock<ImporterBase>();
            string emptyFilePath = "";

            
            Assert.Throws<ArgumentException>(() => new ImportDataCommand(mockImporter.Object, emptyFilePath));
        }

        [Fact]
        public async Task ExecuteAsync_WhenImporterThrowsException_PropagatesException()
        {
            
            var mockImporter = new Mock<ImporterBase>();
            string filePath = "test_import.json";
            var expectedException = new InvalidOperationException("Import failed");

            mockImporter.Setup(i => i.Import(filePath))
                .ThrowsAsync(expectedException);

            var command = new ImportDataCommand(mockImporter.Object, filePath);

            
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => command.ExecuteAsync());
            Assert.Same(expectedException, exception);
        }
    }
}

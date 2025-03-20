using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using FinancialAccounting.DataImportExport.DataImport;

namespace FinancialAccounting.Tests.DataImportExport.DataImport
{
    public class ImporterBaseTests
    {
        private class TestImporter : ImporterBase
        {
            public string ParsedContent { get; private set; }
            
            protected override void Parse(string content)
            {
                ParsedContent = content;
            }
        }
        
        private readonly string _testDirectory;
        
        public ImporterBaseTests()
        {
            
            _testDirectory = Path.Combine(Path.GetTempPath(), "ImporterBaseTests_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
        }
        
        [Fact]
        public async Task Import_WithValidFile_CallsParse()
        {
            
            var filePath = Path.Combine(_testDirectory, "test_data.txt");
            var fileContent = "Test content";
            File.WriteAllText(filePath, fileContent);
            
            var importer = new TestImporter();
            
            
            await importer.Import(filePath);
            
            
            Assert.Equal(fileContent, importer.ParsedContent);
        }
        
        [Fact]
        public async Task Import_WithMissingFile_ThrowsFileNotFoundException()
        {
            
            var filePath = Path.Combine(_testDirectory, "missing_file.txt");
            var importer = new TestImporter();
            
            
            await Assert.ThrowsAsync<FileNotFoundException>(() => importer.Import(filePath));
        }
    }
}

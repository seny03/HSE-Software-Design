using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FinancialAccounting.Domain;
using FinancialAccounting.Persistence;
using FinancialAccounting.DataImportExport.DataExport;

namespace FinancialAccounting.Tests.Commands
{
    public class ExportDataCommandTests
    {
        [Fact]
        public async Task ExecuteAsync_CallsExportVisitorExportToFile()
        {
            
            var mockExportVisitor = new Mock<IExportVisitor>();
            var mockAccountRepo = new Mock<IRepository<BankAccount>>();
            var mockCategoryRepo = new Mock<IRepository<Category>>();
            var mockOperationRepo = new Mock<IRepository<Operation>>();

            string filePath = "test_export.json";

            var command = new ExportDataCommand(
                mockExportVisitor.Object,
                filePath,
                mockAccountRepo.Object,
                mockCategoryRepo.Object,
                mockOperationRepo.Object);

            
            await command.ExecuteAsync();

            
            mockExportVisitor.Verify(v => v.ExportToFile(
                filePath,
                mockAccountRepo.Object,
                mockCategoryRepo.Object,
                mockOperationRepo.Object), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_WithNullExportVisitor_ThrowsArgumentNullException()
        {
            
            IExportVisitor nullVisitor = null;
            var mockAccountRepo = new Mock<IRepository<BankAccount>>();
            var mockCategoryRepo = new Mock<IRepository<Category>>();
            var mockOperationRepo = new Mock<IRepository<Operation>>();

            string filePath = "test_export.json";

            
            Assert.Throws<ArgumentNullException>(() => new ExportDataCommand(
                nullVisitor,
                filePath,
                mockAccountRepo.Object,
                mockCategoryRepo.Object,
                mockOperationRepo.Object));
        }

        [Fact]
        public async Task ExecuteAsync_WithNullFilePath_ThrowsArgumentNullException()
        {
            
            var mockExportVisitor = new Mock<IExportVisitor>();
            var mockAccountRepo = new Mock<IRepository<BankAccount>>();
            var mockCategoryRepo = new Mock<IRepository<Category>>();
            var mockOperationRepo = new Mock<IRepository<Operation>>();

            string nullFilePath = null;

            
            Assert.Throws<ArgumentNullException>(() => new ExportDataCommand(
                mockExportVisitor.Object,
                nullFilePath,
                mockAccountRepo.Object,
                mockCategoryRepo.Object,
                mockOperationRepo.Object));
        }

        [Fact]
        public async Task ExecuteAsync_WithEmptyFilePath_ThrowsArgumentException()
        {
            
            var mockExportVisitor = new Mock<IExportVisitor>();
            var mockAccountRepo = new Mock<IRepository<BankAccount>>();
            var mockCategoryRepo = new Mock<IRepository<Category>>();
            var mockOperationRepo = new Mock<IRepository<Operation>>();

            string emptyFilePath = "";

            
            Assert.Throws<ArgumentException>(() => new ExportDataCommand(
                mockExportVisitor.Object,
                emptyFilePath,
                mockAccountRepo.Object,
                mockCategoryRepo.Object,
                mockOperationRepo.Object));
        }
    }
}

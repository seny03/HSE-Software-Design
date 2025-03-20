using FinancialAccounting.Domain;
using FinancialAccounting.Persistence;

namespace FinancialAccounting.DataImportExport.DataExport
{
    public interface IExportVisitor
    {
        void Visit(BankAccount account);
        void Visit(Category category);
        void Visit(Operation operation);

        void ExportToFile(string filePath, IRepository<BankAccount> accRepo, IRepository<Category> catRepo, IRepository<Operation> opRepo);
    }
}

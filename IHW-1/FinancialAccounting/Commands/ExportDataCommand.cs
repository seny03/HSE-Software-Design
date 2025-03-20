using System;
using System.Threading.Tasks;
using FinancialAccounting.DataImportExport.DataExport;
using FinancialAccounting.Persistence;
using FinancialAccounting.Domain;

public class ExportDataCommand : ICommand
{
    private readonly IExportVisitor _exportVisitor;
    private readonly string _filePath;
    private readonly IRepository<BankAccount> _accRepo;
    private readonly IRepository<Category> _catRepo;
    private readonly IRepository<Operation> _opRepo;

    public ExportDataCommand(IExportVisitor exportVisitor, string filePath,
        IRepository<BankAccount> accRepo, IRepository<Category> catRepo, IRepository<Operation> opRepo)
    {
        if (exportVisitor == null)
            throw new ArgumentNullException(nameof(exportVisitor));
            
        if (filePath == null)
            throw new ArgumentNullException(nameof(filePath));
            
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty or whitespace", nameof(filePath));
            
        if (accRepo == null)
            throw new ArgumentNullException(nameof(accRepo));
            
        if (catRepo == null)
            throw new ArgumentNullException(nameof(catRepo));
            
        if (opRepo == null)
            throw new ArgumentNullException(nameof(opRepo));
            
        _exportVisitor = exportVisitor;
        _filePath = filePath;
        _accRepo = accRepo;
        _catRepo = catRepo;
        _opRepo = opRepo;
    }

    public async Task ExecuteAsync()
    {
        _exportVisitor.ExportToFile(_filePath, _accRepo, _catRepo, _opRepo);
        Console.WriteLine($"Exported data to {_filePath}");
        await Task.CompletedTask;
    }
}

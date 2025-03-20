using System;
using System.Threading.Tasks;
using FinancialAccounting.DataImportExport.DataImport;

public class ImportDataCommand : ICommand
{
    private readonly ImporterBase _importer;
    private readonly string _filePath;

    public ImportDataCommand(ImporterBase importer, string filePath)
    {
        if (importer == null)
            throw new ArgumentNullException(nameof(importer));
        
        if (filePath == null)
            throw new ArgumentNullException(nameof(filePath));
            
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty or whitespace", nameof(filePath));
            
        _importer = importer;
        _filePath = filePath;
    }

    public async Task ExecuteAsync()
    {
        await _importer.Import(_filePath);
        Console.WriteLine($"Imported data from {_filePath}");
    }
}

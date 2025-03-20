using System;
using System.Linq;
using System.Threading.Tasks;
using FinancialAccounting.Services;

public class ReportCategoryCommand : ICommand
{
    private readonly OperationService _operationService;
    private readonly CategoryService _categoryService;
    private readonly Guid _accountId;

    public ReportCategoryCommand(OperationService operationService, CategoryService categoryService, Guid accountId)
    {
        _operationService = operationService;
        _categoryService = categoryService;
        _accountId = accountId;
    }

    public async Task ExecuteAsync()
    {
        var groupedData = _operationService.GroupByCategory(_accountId, _categoryService);

        Console.WriteLine($"Grouped expenses/incomes for account {_accountId}:");
        foreach (var (category, amount) in groupedData)
        {
            Console.WriteLine($"{category}: {amount}");
        }

        await Task.CompletedTask;
    }
}

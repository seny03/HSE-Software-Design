using System;
using System.Threading.Tasks;
using FinancialAccounting.Services;

public class ReportBalanceCommand : ICommand
{
    private readonly OperationService _operationService;
    private readonly Guid _accountId;
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;

    public ReportBalanceCommand(OperationService operationService, Guid accountId, DateTime startDate, DateTime endDate)
    {
        _operationService = operationService;
        _accountId = accountId;
        _startDate = startDate;
        _endDate = endDate;
    }

    public async Task ExecuteAsync()
    {
        var balanceDifference = _operationService.BalanceDifference(_accountId, _startDate, _endDate);
        Console.WriteLine($"Balance difference for account {_accountId} from {_startDate:yyyy-MM-dd} to {_endDate:yyyy-MM-dd}: {balanceDifference}");
        await Task.CompletedTask;
    }
}

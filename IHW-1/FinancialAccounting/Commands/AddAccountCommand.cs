using System;
using System.Threading.Tasks;
using FinancialAccounting.Domain;
using FinancialAccounting.Services;

public class AddAccountCommand : ICommand
{
    private readonly AccountService _service;
    private readonly string _name;
    private readonly decimal _balance;

    public AddAccountCommand(AccountService service, string name, decimal balance)
    {
        _service = service;
        _name = name;
        _balance = balance;
    }

    public async Task ExecuteAsync()
    {
        var account = _service.Create(_name, _balance);
        Console.WriteLine($"Created Account: {account.Id}");
        await Task.CompletedTask;
    }
}

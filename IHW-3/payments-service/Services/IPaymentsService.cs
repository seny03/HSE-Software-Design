using PaymentsService.Models;

namespace PaymentsService.Services;

public interface IPaymentsService
{
    Task<Account> CreateAccountAsync(string userName);
    Task<Account?> DepositAsync(Guid userId, decimal amount);
    Task<decimal?> GetBalanceAsync(Guid userId);
    Task<(bool, string)> WithdrawAsync(Guid userId, decimal amount);
    Task<List<Account>> GetAllAccountsAsync();
} 
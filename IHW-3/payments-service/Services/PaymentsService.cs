using Microsoft.EntityFrameworkCore;
using PaymentsService.Data;
using PaymentsService.Models;

namespace PaymentsService.Services;

public class PaymentsService : IPaymentsService
{
    private readonly PaymentsDbContext _context;
    private readonly ILogger<PaymentsService> _logger;

    public PaymentsService(PaymentsDbContext context, ILogger<PaymentsService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Account> CreateAccountAsync(string userName)
    {
        var account = new Account
        {
            UserName = userName,
            Balance = 0
        };
        
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task<Account?> DepositAsync(Guid userId, decimal amount)
    {
        if (amount <= 0)
        {
            _logger.LogWarning("Deposit amount must be positive for user {UserId}", userId);
            return null; 
        }
        
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);
        if (account == null)
        {
            return null;
        }

        account.Balance += amount;
        
        var transaction = new Transaction
        {
            AccountId = account.UserId,
            Amount = amount,
            Type = TransactionType.Deposit,
            Timestamp = DateTime.UtcNow
        };
        _context.Transactions.Add(transaction);

        await _context.SaveChangesAsync();
        return account;
    }

    public async Task<decimal?> GetBalanceAsync(Guid userId)
    {
        var account = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.UserId == userId);
        return account?.Balance;
    }

    public async Task<(bool, string)> WithdrawAsync(Guid userId, decimal amount)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);
        if (account == null)
        {
            return (false, "Account not found");
        }

        if (account.Balance < amount)
        {
            return (false, "Insufficient funds");
        }

        account.Balance -= amount;
        
        var transaction = new Transaction
        {
            AccountId = account.UserId,
            Amount = -amount,
            Type = TransactionType.Withdrawal,
            Timestamp = DateTime.UtcNow
        };
        _context.Transactions.Add(transaction);
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Withdrawal of {Amount} for user {UserId} successful", amount, userId);
        return (true, "Withdrawal successful");
    }

    public async Task<List<Account>> GetAllAccountsAsync()
    {
        return await _context.Accounts.AsNoTracking().ToListAsync();
    }
} 
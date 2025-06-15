namespace PaymentsService.Models;

public class Account
{
    public Guid UserId { get; set; } = Guid.NewGuid();
    public required string UserName { get; set; }
    public decimal Balance { get; set; }
    public List<Transaction> Transactions { get; set; } = new();
} 
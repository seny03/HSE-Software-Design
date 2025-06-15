namespace PaymentsService.Models;

public class Transaction
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public DateTime Timestamp { get; set; }
}

public enum TransactionType
{
    Deposit,
    Withdrawal
} 
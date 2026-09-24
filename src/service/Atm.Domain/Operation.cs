namespace Atm.Domain;

public enum OperationType // только списание или депозит
{
    Withdraw,
    Deposit,
}

public class Operation
{
    public string AccountNumber { get; }

    public OperationType Type { get; }

    public decimal Amount { get; }

    public DateTime Timestamp { get; }

    public Operation(string accountNumber, OperationType type, decimal amount, DateTime? timestamp = null)
    {
        AccountNumber = accountNumber;
        Type = type;
        Amount = amount;
        Timestamp = timestamp ?? DateTime.UtcNow;
    }
}
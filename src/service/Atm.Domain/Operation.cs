namespace Atm.Domain;

public enum OperationType // только списание или депозит
{
    Withdraw,
    Deposit,
}

public class Operation
{
    public long Id { get; }

    public string AccountNumber { get; }

    public OperationType Type { get; }

    public decimal Amount { get; }

    public Guid? InvoiceId { get; }

    public DateTime Timestamp { get; }

    public Operation(
        string accountNumber,
        OperationType type,
        decimal amount,
        Guid? invoiceId = null,
        DateTime? timestamp = null,
        long id = 0)
    {
        Id = id;
        AccountNumber = accountNumber;
        Type = type;
        Amount = amount;
        InvoiceId = invoiceId;
        Timestamp = timestamp ?? DateTime.UtcNow;
    }
}

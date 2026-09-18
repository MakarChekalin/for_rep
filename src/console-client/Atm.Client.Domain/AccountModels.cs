namespace Atm.Client.Domain;

public class SessionKeyResponse
{
    public Guid SessionKey { get; set; }
}

public class BalanceResponse
{
    public decimal Balance { get; set; }
}

public class OperationResponse
{
    public string AccountNumber { get; set; } = string.Empty;
    public int Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; }
}
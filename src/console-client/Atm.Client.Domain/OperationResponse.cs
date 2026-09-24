namespace Atm.Client.Domain;

public class OperationResponse
{
    public string AccountNumber { get; set; } = string.Empty;

    public int Type { get; set; }

    public decimal Amount { get; set; }

    public DateTime Timestamp { get; set; }
}
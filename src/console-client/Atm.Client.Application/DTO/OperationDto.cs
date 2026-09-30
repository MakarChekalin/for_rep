namespace Atm.Client.Application.DTO;

public class OperationDto
{
    public string AccountNumber { get; set; } = string.Empty;

    public int Type { get; set; }

    public decimal Amount { get; set; }

    public DateTime Timestamp { get; set; }
}

namespace Atm.Gateway.DTO;

public record OperationDto(string Type, decimal Amount, Guid? InvoiceId, DateTime Timestamp);

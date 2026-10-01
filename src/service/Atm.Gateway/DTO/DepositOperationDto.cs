namespace Atm.Gateway.DTO;

public record DepositOperationDto(decimal Amount, DateTime Timestamp, Guid? InvoiceId) : OperationDto(Amount, Timestamp);

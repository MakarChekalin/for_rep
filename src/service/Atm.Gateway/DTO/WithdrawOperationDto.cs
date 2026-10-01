namespace Atm.Gateway.DTO;

public record WithdrawOperationDto(decimal Amount, DateTime Timestamp, Guid? InvoiceId) : OperationDto(Amount, Timestamp);

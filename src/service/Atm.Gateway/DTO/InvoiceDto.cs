namespace Atm.Gateway.DTO;

public record InvoiceDto(
    Guid Id,
    string PayerAccountNumber,
    string PayeeAccountNumber,
    decimal Amount,
    string Status,
    DateTime CreatedAt);

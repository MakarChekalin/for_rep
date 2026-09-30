namespace Atm.Domain;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(Guid id);

    Task SaveAsync(Invoice invoice);

    IAsyncEnumerable<Invoice> GetOutgoingAsync(string payeeAccountNumber, string? payerAccountNumber, InvoiceStatus? status, Guid? cursor);

    IAsyncEnumerable<Invoice> GetIncomingAsync(string payerAccountNumber, string? payeeAccountNumber, InvoiceStatus? status, Guid? cursor);
}

using Atm.Application.Results;
using Atm.Domain;

namespace Atm.Application;

public interface IInvoiceService
{
    Task<CreateInvoiceResult> CreateInvoiceAsync(Guid sessionKey, string payerAccountNumber, decimal amount);

    Task<PayInvoiceResult> PayInvoiceAsync(Guid sessionKey, Guid invoiceId);

    Task<CancelInvoiceResult> CancelInvoiceAsync(Guid sessionKey, Guid invoiceId);

    Task<GetOutgoingInvoicesResult> GetOutgoingInvoicesAsync(Guid sessionKey, string? payerAccountNumber, InvoiceStatus? status, Guid? cursor, int pageSize);

    Task<GetIncomingInvoicesResult> GetIncomingInvoicesAsync(Guid sessionKey, string? payeeAccountNumber, InvoiceStatus? status, Guid? cursor, int pageSize);
}

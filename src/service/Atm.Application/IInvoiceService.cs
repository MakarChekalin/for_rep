using Atm.Application.Results;
using Atm.Domain;

namespace Atm.Application;

public interface IInvoiceService
{
    Task<CreateInvoiceResult> CreateInvoiceAsync(Guid sessionKey, string payerAccountNumber, decimal amount, string userId);

    Task<PayInvoiceResult> PayInvoiceAsync(Guid sessionKey, Guid invoiceId, string userId);

    Task<CancelInvoiceResult> CancelInvoiceAsync(Guid sessionKey, Guid invoiceId, string userId);

    Task<GetOutgoingInvoicesResult> GetOutgoingInvoicesAsync(Guid sessionKey, string? payerAccountNumber, InvoiceStatus? status, Guid? cursor, int pageSize, string userId);

    Task<GetIncomingInvoicesResult> GetIncomingInvoicesAsync(Guid sessionKey, string? payeeAccountNumber, InvoiceStatus? status, Guid? cursor, int pageSize, string userId);

    Task<AssignAccountantResult> AssignAccountantAsync(Guid sessionKey, Guid invoiceId, string accountantUserId, string userId);

    Task<AccountantActionResult> ApproveInvoiceAsync(Guid sessionKey, Guid invoiceId, string userId);

    Task<AccountantActionResult> DeclineInvoiceAsync(Guid sessionKey, Guid invoiceId, string userId);
}

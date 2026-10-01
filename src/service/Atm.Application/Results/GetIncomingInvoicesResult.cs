using Atm.Domain;

namespace Atm.Application.Results;

public record GetIncomingInvoicesResult(GetInvoicesStatus Status, IReadOnlyList<Invoice>? Invoices, Guid? NextCursor);

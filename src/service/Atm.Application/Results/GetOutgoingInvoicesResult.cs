using Atm.Domain;

namespace Atm.Application.Results;

public record GetOutgoingInvoicesResult(GetInvoicesStatus Status, IReadOnlyList<Invoice>? Invoices, Guid? NextCursor);

using Atm.Domain;

namespace Atm.Application.Results;

public record GetOutgoingInvoicesResult(GetInvoicesStatus Status, IAsyncEnumerable<Invoice>? Invoices);

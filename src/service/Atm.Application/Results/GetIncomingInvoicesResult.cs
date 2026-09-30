using Atm.Domain;

namespace Atm.Application.Results;

public record GetIncomingInvoicesResult(GetInvoicesStatus Status, IAsyncEnumerable<Invoice>? Invoices);

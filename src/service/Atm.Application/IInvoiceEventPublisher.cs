namespace Atm.Application;

public interface IInvoiceEventPublisher
{
    Task PublishInvoiceCreatedAsync(long invoiceExternalId, long payeeExternalId, long payerExternalId, decimal amount, CancellationToken cancellationToken);
}

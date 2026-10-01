using Atm.Application;
using Invoices.Kafka.Contracts;
using Itmo.Dev.Platform.Kafka.Producer;

namespace Atm.Grpc.Kafka;

public class InvoiceEventPublisher : IInvoiceEventPublisher
{
    private readonly IKafkaMessageProducer<InvoiceCreationKey, InvoiceCreationValue> _producer;

    public InvoiceEventPublisher(IKafkaMessageProducer<InvoiceCreationKey, InvoiceCreationValue> producer)
    {
        _producer = producer;
    }

    public async Task PublishInvoiceCreatedAsync(long invoiceExternalId, long payeeExternalId, long payerExternalId, decimal amount, CancellationToken cancellationToken)
    {
        var key = new InvoiceCreationKey { InvoiceId = invoiceExternalId };

        var value = new InvoiceCreationValue
        {
            InvoiceId = invoiceExternalId,
            RecipientId = payeeExternalId,
            PayerId = payerExternalId,
            Payment = MoneyMapping.ToMoney(amount),
        };

        var message = new KafkaProducerMessage<InvoiceCreationKey, InvoiceCreationValue>(Key: key, Value: value);

        await _producer.ProduceAsync(ToAsyncEnumerable(message), cancellationToken);
    }

    private static async IAsyncEnumerable<KafkaProducerMessage<InvoiceCreationKey, InvoiceCreationValue>> ToAsyncEnumerable(
        KafkaProducerMessage<InvoiceCreationKey, InvoiceCreationValue> message)
    {
        yield return message;
        await Task.CompletedTask;
    }
}

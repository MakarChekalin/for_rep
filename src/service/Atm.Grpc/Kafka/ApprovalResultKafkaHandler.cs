using ApprovalResult.Kafka.Contracts;
using Atm.Domain;
using Itmo.Dev.Platform.Kafka.Consumer;

namespace Atm.Grpc.Kafka;

public class ApprovalResultKafkaHandler : IKafkaConsumerHandler<ApprovalResultKey, ApprovalResultValue>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ILogger<ApprovalResultKafkaHandler> _logger;

    public ApprovalResultKafkaHandler(IInvoiceRepository invoiceRepository, ILogger<ApprovalResultKafkaHandler> logger)
    {
        _invoiceRepository = invoiceRepository;
        _logger = logger;
    }

    public async ValueTask HandleAsync(
        IEnumerable<IKafkaConsumerMessage<ApprovalResultKey, ApprovalResultValue>> messages,
        CancellationToken cancellationToken)
    {
        foreach (IKafkaConsumerMessage<ApprovalResultKey, ApprovalResultValue> message in messages)
            await HandleAsync(message.Value);
    }

    private async Task HandleAsync(ApprovalResultValue value)
    {
        Atm.Domain.Invoice? invoice = await _invoiceRepository.GetByExternalIdAsync(value.InvoiceId);

        if (invoice == null)
        {
            _logger.LogWarning("Approval result for unknown invoice {InvoiceExternalId} ignored", value.InvoiceId);
            return;
        }

        bool applied = value.Status switch
        {
            ApprovalStatus.Approved => invoice.Approve(),
            ApprovalStatus.Declined => invoice.Decline(),
            ApprovalStatus.Unspecified => false,
            _ => false,
        };

        if (!applied)
        {
            _logger.LogWarning(
                "Approval result {Status} for invoice {InvoiceExternalId} could not be applied from state {State}",
                value.Status,
                value.InvoiceId,
                invoice.Status);

            return;
        }

        await _invoiceRepository.SaveAsync(invoice);
    }
}

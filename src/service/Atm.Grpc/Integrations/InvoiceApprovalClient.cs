using Atm.Application;
using Grpc.Core;

namespace Atm.Grpc.Integrations;

public class InvoiceApprovalClient : IInvoiceApprovalClient
{
    private readonly Invoices.Grpc.Contracts.InvoiceService.InvoiceServiceClient _client;

    public InvoiceApprovalClient(Invoices.Grpc.Contracts.InvoiceService.InvoiceServiceClient client)
    {
        _client = client;
    }

    public Task<InvoiceApprovalCallResult> AssignAccountantAsync(long invoiceExternalId, long accountantExternalId, CancellationToken cancellationToken)
    {
        return CallAsync(() => _client.AssignAccountantAsync(
            new Invoices.Grpc.Contracts.AssignAccountantRequest { InvoiceId = invoiceExternalId, UserId = accountantExternalId },
            cancellationToken: cancellationToken).ResponseAsync);
    }

    public Task<InvoiceApprovalCallResult> ApproveInvoiceAsync(long invoiceExternalId, long accountantExternalId, CancellationToken cancellationToken)
    {
        return CallAsync(() => _client.ApproveInvoiceAsync(
            new Invoices.Grpc.Contracts.ApproveInvoiceRequest { InvoiceId = invoiceExternalId, UserId = accountantExternalId },
            cancellationToken: cancellationToken).ResponseAsync);
    }

    public Task<InvoiceApprovalCallResult> DeclineInvoiceAsync(long invoiceExternalId, long accountantExternalId, CancellationToken cancellationToken)
    {
        return CallAsync(() => _client.DeclineInvoiceAsync(
            new Invoices.Grpc.Contracts.DeclineInvoiceRequest { InvoiceId = invoiceExternalId, UserId = accountantExternalId },
            cancellationToken: cancellationToken).ResponseAsync);
    }

    private static async Task<InvoiceApprovalCallResult> CallAsync<TResponse>(Func<Task<TResponse>> call)
    {
        try
        {
            await call();
            return InvoiceApprovalCallResult.Success;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            return InvoiceApprovalCallResult.NotFound;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.FailedPrecondition)
        {
            return InvoiceApprovalCallResult.InvalidState;
        }
        catch (RpcException)
        {
            return InvoiceApprovalCallResult.ServiceUnavailable;
        }
    }
}

using Atm.Application;
using Microsoft.Extensions.Options;

namespace Atm.Grpc.Integrations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInvoiceApprovalIntegration(this IServiceCollection services)
    {
        services.AddOptions<InvoiceApprovalClientOptions>().BindConfiguration("InvoiceApprovalService");

        services.AddGrpcClient<Invoices.Grpc.Contracts.InvoiceService.InvoiceServiceClient>((provider, options) =>
        {
            IOptions<InvoiceApprovalClientOptions> clientOptions = provider.GetRequiredService<IOptions<InvoiceApprovalClientOptions>>();
            options.Address = new Uri(clientOptions.Value.BaseUrl);
        });

        services.AddScoped<IInvoiceApprovalClient, InvoiceApprovalClient>();

        return services;
    }
}

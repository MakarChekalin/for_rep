using Itmo.Dev.Platform.Options;

namespace Atm.Grpc.Integrations;

[OptionsType]
public sealed class InvoiceApprovalClientOptions
{
    public string BaseUrl { get; set; } = string.Empty;
}

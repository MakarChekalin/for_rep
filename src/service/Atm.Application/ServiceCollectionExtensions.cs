using Microsoft.Extensions.DependencyInjection;

namespace Atm.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IInvoiceService, InvoiceService>();

        return services;
    }
}

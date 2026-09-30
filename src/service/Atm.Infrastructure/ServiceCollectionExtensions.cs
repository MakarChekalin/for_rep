using Atm.Application;
using Atm.Domain;
using Atm.Infrastructure.Migrations;
using Itmo.Dev.Platform.Common.Extensions;
using Itmo.Dev.Platform.Persistence.Abstractions.Extensions;
using Itmo.Dev.Platform.Persistence.Postgres.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Atm.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddOptions<AdminOptions>()
            .Configure<IConfiguration>((options, configuration) =>
                options.Password = configuration["AdminPassword"] ?? string.Empty)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddPlatform(serializer => serializer.WithNewtonsoftSerialization());

        services.AddPlatformPersistence(persistence => persistence.UsePostgres(postgres => postgres
            .WithConnectionOptions("Infrastructure:Persistence:Postgres")
            .WithMigrationsFrom(typeof(Migration001InitialSchema).Assembly)));

        services.AddScoped<IAccountRepository, PostgresAccountRepository>();
        services.AddScoped<ISessionRepository, PostgresSessionRepository>();
        services.AddScoped<IOperationRepository, PostgresOperationRepository>();
        services.AddScoped<IInvoiceRepository, PostgresInvoiceRepository>();
        services.AddSingleton<IAdminPasswordValidator, AdminPasswordValidator>();

        return services;
    }
}

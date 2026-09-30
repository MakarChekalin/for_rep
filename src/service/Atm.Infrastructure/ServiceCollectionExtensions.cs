using Atm.Application;
using Atm.Domain;
using Atm.Infrastructure.Migrations;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Atm.Infrastructure;

public static class ServiceCollectionExtensions
{
    // конфигурации читаются из IConfiguration и отдаются через IOptions с валидацией при старте
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddOptions<DatabaseOptions>()
            .Configure<IConfiguration>((options, configuration) =>
                options.ConnectionString = configuration.GetConnectionString("Postgres") ?? string.Empty)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<AdminOptions>()
            .Configure<IConfiguration>((options, configuration) =>
                options.Password = configuration["AdminPassword"] ?? string.Empty)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton(serviceProvider =>
            NpgsqlDataSource.Create(serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value.ConnectionString));

        services.AddSingleton<IAccountRepository, PostgresAccountRepository>();
        services.AddSingleton<ISessionRepository, PostgresSessionRepository>();
        services.AddSingleton<IOperationRepository, PostgresOperationRepository>();
        services.AddSingleton<IAdminPasswordValidator, AdminPasswordValidator>();

        services.AddFluentMigratorCore() // настройка миграции(куда подключаться)
            .ConfigureRunner(rb => rb
                .AddPostgres() // что за бд
                .WithGlobalConnectionString(serviceProvider =>
                    serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value.ConnectionString) // куда подкл.
                .ScanIn(typeof(Migration001InitialSchema).Assembly).For.Migrations()); // тут ищем все классы миграции

        return services;
    }
}

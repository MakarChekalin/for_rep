using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Atm.Infrastructure;

public static class MigrationExtensions
{
    public static IHost MigrateDatabase(this IHost host)
    {
        using (IServiceScope scope = host.Services.CreateScope()) // применение миграций при страрте
        {
            IMigrationRunner runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }

        return host;
    }
}

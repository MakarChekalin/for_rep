using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

namespace Atm.Infrastructure.Migrations;

[Migration(3)]
public class Migration003AddOperationPayload : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider)
    {
        return """
            alter table operations add column payload jsonb;

            update operations
            set payload = json_build_object('amount', amount);

            alter table operations alter column payload set not null;
            """;
    }

    protected override string GetDownSql(IServiceProvider serviceProvider)
    {
        return """
            alter table operations drop column payload;
            """;
    }
}

using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

namespace Atm.Infrastructure.Migrations;

[Migration(8)]
public class Migration008AddExternalIds : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider)
    {
        return """
            alter table users add column external_id bigserial unique;
            alter table invoices add column external_id bigserial unique;
            alter table accounts add column external_id bigserial unique;
            """;
    }

    protected override string GetDownSql(IServiceProvider serviceProvider)
    {
        return """
            alter table users drop column external_id;
            alter table invoices drop column external_id;
            alter table accounts drop column external_id;
            """;
    }
}

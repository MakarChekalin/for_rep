using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

namespace Atm.Infrastructure.Migrations;

[Migration(5)]
public class Migration005AccountUserIdNotNull : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider)
    {
        return """
            alter table accounts alter column user_id set not null;
            """;
    }

    protected override string GetDownSql(IServiceProvider serviceProvider)
    {
        return """
            alter table accounts alter column user_id drop not null;
            """;
    }
}

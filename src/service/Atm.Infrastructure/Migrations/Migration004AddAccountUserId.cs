using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

namespace Atm.Infrastructure.Migrations;

[Migration(4)]
public class Migration004AddAccountUserId : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider)
    {
        return """
            alter table accounts add column user_id varchar(255) null;
            """;
    }

    protected override string GetDownSql(IServiceProvider serviceProvider)
    {
        return """
            alter table accounts drop column user_id;
            """;
    }
}

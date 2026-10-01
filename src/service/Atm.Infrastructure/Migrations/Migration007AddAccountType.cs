using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

namespace Atm.Infrastructure.Migrations;

[Migration(7)]
public class Migration007AddAccountType : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider)
    {
        return """
            alter table accounts add column account_type varchar(20) not null default 'Personal';
            """;
    }

    protected override string GetDownSql(IServiceProvider serviceProvider)
    {
        return """
            alter table accounts drop column account_type;
            """;
    }
}

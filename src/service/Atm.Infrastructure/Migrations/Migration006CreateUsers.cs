using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

namespace Atm.Infrastructure.Migrations;

[Migration(6)]
public class Migration006CreateUsers : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider)
    {
        return """
            create table users
            (
                id varchar(255) primary key,
                created_at timestamp not null
            );
            """;
    }

    protected override string GetDownSql(IServiceProvider serviceProvider)
    {
        return """
            drop table users;
            """;
    }
}

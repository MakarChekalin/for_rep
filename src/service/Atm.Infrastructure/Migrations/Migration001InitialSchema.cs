using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

namespace Atm.Infrastructure.Migrations;

[Migration(1)]
public class Migration001InitialSchema : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider)
    {
        return """
            create table accounts
            (
                number varchar(50) primary key,
                pin_code varchar(50) not null,
                balance decimal not null
            );

            create table sessions
            (
                key uuid primary key,
                type varchar(20) not null,
                account_number varchar(50) null
            );

            create table operations
            (
                id serial primary key,
                account_number varchar(50) not null,
                type varchar(20) not null,
                amount decimal not null,
                timestamp timestamp not null
            );
            """;
    }

    protected override string GetDownSql(IServiceProvider serviceProvider)
    {
        return """
            drop table operations;
            drop table sessions;
            drop table accounts;
            """;
    }
}

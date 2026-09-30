using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

namespace Atm.Infrastructure.Migrations;

[Migration(2)]
public class Migration002CreateInvoices : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider)
    {
        return """
            create table invoices
            (
                id uuid primary key,
                payer_account_number varchar(50) not null,
                payee_account_number varchar(50) not null,
                amount decimal not null,
                status varchar(20) not null,
                created_at timestamp not null
            );
            """;
    }

    protected override string GetDownSql(IServiceProvider serviceProvider)
    {
        return """
            drop table invoices;
            """;
    }
}

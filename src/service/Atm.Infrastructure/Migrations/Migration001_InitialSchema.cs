using FluentMigrator;

namespace Atm.Infrastructure.Migrations;

[Migration(1)] // показывает, что эту миграцию надо делать первой
public class Migration001_InitialSchema : Migration
{
    public override void Up() // создание таблиц
    {
        Execute.Sql(@"
            CREATE TABLE accounts (
                number VARCHAR(50) PRIMARY KEY, 
                pin_code VARCHAR(50) NOT NULL,
                balance DECIMAL NOT NULL
            );
        ");
//UUID то же самое что и Guid
        Execute.Sql(@"
            CREATE TABLE sessions (
                key UUID PRIMARY KEY,
                type VARCHAR(20) NOT NULL,
                account_number VARCHAR(50) NULL
            );
        ");
// SERIAL сам увеличивает число
        Execute.Sql(@"
            CREATE TABLE operations (
                id SERIAL PRIMARY KEY,
                account_number VARCHAR(50) NOT NULL,
                type VARCHAR(20) NOT NULL,
                amount DECIMAL NOT NULL,
                timestamp TIMESTAMP NOT NULL
            );
        ");
    }

    public override void Down() // откат таблиц 
    {
        Execute.Sql("DROP TABLE operations;");
        Execute.Sql("DROP TABLE sessions;");
        Execute.Sql("DROP TABLE accounts;");
    }
}
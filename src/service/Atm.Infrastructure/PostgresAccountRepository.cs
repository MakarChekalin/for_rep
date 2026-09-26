using Atm.Domain;
using Npgsql;

namespace Atm.Infrastructure;

public class PostgresAccountRepository : IAccountRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public PostgresAccountRepository(NpgsqlDataSource ds)
    {
        _dataSource = ds;
    }

    public async Task<Account?> GetByNumberAsync(string number)
    {
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();

        await using var command = new NpgsqlCommand(
            "SELECT number, pin_code, balance FROM accounts WHERE number = @number",
            connection);
        command.Parameters.AddWithValue("number", number); // делаем так для защиты

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return new Account(
            reader.GetString(reader.GetOrdinal("number")),
            reader.GetString(reader.GetOrdinal("pin_code")),
            reader.GetDecimal(reader.GetOrdinal("balance")));
    }

    public async Task SaveAsync(Account account)
    {
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();

        // тут ON CONFLICT нужен чтобы при конфликте с PR баланс обновлялся
        await using var command = new NpgsqlCommand(
            @"
            INSERT INTO accounts (number, pin_code, balance)
            VALUES (@number, @pinCode, @balance)
            ON CONFLICT (number) DO UPDATE SET balance = @balance",
            connection);

        command.Parameters.AddWithValue("number", account.Number);
        command.Parameters.AddWithValue("pinCode", account.PinCode);
        command.Parameters.AddWithValue("balance", account.Balance);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<bool> ExistsAsync(string number)
    {
        Account? account = await GetByNumberAsync(number);
        return account != null;
    }
}
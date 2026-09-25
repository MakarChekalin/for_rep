using Atm.Domain;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Atm.Infrastructure;

public class PostgresAccountRepository : IAccountRepository
{
    private readonly string _connectionString;

    public PostgresAccountRepository(IOptions<DatabaseOptions> options)
    {
        _connectionString = options.Value.ConnectionString;
    }

    public async Task<Account?> GetByNumberAsync(string number)
    {
        await using var connection = new NpgsqlConnection(_connectionString); // тут создается соединение (connection берется из настроек)
        await connection.OpenAsync(); // открытие соединения

        await using var command = new NpgsqlCommand(
            "SELECT number, pin_code, balance FROM accounts WHERE number = @number",
            connection);
        command.Parameters.AddWithValue("number", number); // делаем так для защиты

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return new Account(
            reader.GetString(0),
            reader.GetString(1),
            reader.GetDecimal(2));
    }

    public async Task SaveAsync(Account account)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

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
        var account = await GetByNumberAsync(number);
        return account != null;
    }
}
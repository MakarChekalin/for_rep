using Atm.Domain;
using Npgsql;

namespace Atm.Infrastructure;

public class PostgresOperationRepository : IOperationRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public PostgresOperationRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task SaveAsync(Operation operation)
    {
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();

        await using var command = new NpgsqlCommand(
            @"
            INSERT INTO operations (account_number, type, amount, timestamp)
            VALUES (@accountNumber, @type, @amount, @timestamp)",
            connection);

        command.Parameters.AddWithValue("accountNumber", operation.AccountNumber);
        command.Parameters.AddWithValue("type", operation.Type.ToString());
        command.Parameters.AddWithValue("amount", operation.Amount);
        command.Parameters.AddWithValue("timestamp", operation.Timestamp);

        await command.ExecuteNonQueryAsync();
    }

    // IAsyncEnumerable: операции отдаются по одной по мере чтения из БД, без накопления всего списка в памяти
    public async IAsyncEnumerable<Operation> GetByAccountNumberAsync(string accountNumber)
    {
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();

        await using var command = new NpgsqlCommand(
            "SELECT account_number, type, amount, timestamp FROM operations WHERE account_number = @accountNumber ORDER BY timestamp",
            connection); // сортировка по времени
        command.Parameters.AddWithValue("accountNumber", accountNumber);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            OperationType type = Enum.Parse<OperationType>(reader.GetString(reader.GetOrdinal("type")));

            yield return new Operation(
                reader.GetString(reader.GetOrdinal("account_number")),
                type,
                reader.GetDecimal(reader.GetOrdinal("amount")),
                reader.GetDateTime(reader.GetOrdinal("timestamp")));
        }
    }
}
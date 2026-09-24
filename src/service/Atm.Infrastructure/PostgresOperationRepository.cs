using Atm.Domain;
using Npgsql;

namespace Atm.Infrastructure;

public class PostgresOperationRepository : IOperationRepository
{
    private readonly string _connectionString;

    public PostgresOperationRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task SaveAsync(Operation operation)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

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

    public async Task<IReadOnlyList<Operation>> GetByAccountNumberAsync(string accountNumber)
    {
        var results = new List<Operation>();

        await foreach (var operation in StreamByAccountNumberAsync(accountNumber))
        {
            results.Add(operation);
        }

        return results;
    }

    private async IAsyncEnumerable<Operation> StreamByAccountNumberAsync(string accountNumber) // тут используем async list(по условию)
    { // + async тут нужен так как тут ищутся все операции по аккаунту(и что бы их все найти в дб нужно много времени)
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            "SELECT account_number, type, amount, timestamp FROM operations WHERE account_number = @accountNumber ORDER BY timestamp",
            connection); // сортировка по времени
        command.Parameters.AddWithValue("accountNumber", accountNumber);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var type = Enum.Parse<OperationType>(reader.GetString(1));

            yield return new Operation(
                reader.GetString(0),
                type,
                reader.GetDecimal(2),
                reader.GetDateTime(3));
        }
    }
}
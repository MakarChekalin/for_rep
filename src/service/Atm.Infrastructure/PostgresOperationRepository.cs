using Atm.Domain;
using Itmo.Dev.Platform.Common.Serialization;
using Itmo.Dev.Platform.Persistence.Abstractions.Commands;
using Itmo.Dev.Platform.Persistence.Abstractions.Connections;
using Itmo.Dev.Platform.Persistence.Postgres.Extensions;
using System.Data.Common;

namespace Atm.Infrastructure;

public class PostgresOperationRepository : IOperationRepository
{
    private readonly IPersistenceConnectionProvider _connectionProvider;
    private readonly IPlatformSerializer _serializer;

    public PostgresOperationRepository(IPersistenceConnectionProvider connectionProvider, IPlatformSerializer serializer)
    {
        _connectionProvider = connectionProvider;
        _serializer = serializer;
    }

    public async Task SaveAsync(Operation operation)
    {
        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(CancellationToken.None);

        var payload = new OperationPayload(operation.Amount, operation.InvoiceId);

        await using IPersistenceCommand command = connection
            .CreateCommand("""
                insert into operations (account_number, type, amount, timestamp, payload)
                values (@accountNumber, @type, @amount, @timestamp, @payload)
                """)
            .AddParameter<string>("accountNumber", operation.AccountNumber)
            .AddParameter<string>("type", operation.Type.ToString())
            .AddParameter("amount", operation.Amount)
            .AddParameter("timestamp", operation.Timestamp)
            .AddJsonParameter("payload", payload);

        await command.ExecuteNonQueryAsync(CancellationToken.None);
    }

    public async IAsyncEnumerable<Operation> GetByAccountNumberAsync(string accountNumber, long? cursor, int pageSize)
    {
        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(CancellationToken.None);

        await using IPersistenceCommand command = connection
            .CreateCommand("""
                select id, account_number, type, amount, timestamp, payload
                from operations
                where account_number = @accountNumber
                  and (@cursor is null or id > @cursor)
                order by id
                limit @pageSize
                """)
            .AddParameter<string>("accountNumber", accountNumber)
            .AddParameter<long?>("cursor", cursor)
            .AddParameter("pageSize", pageSize);

        await using DbDataReader reader = await command.ExecuteReaderAsync(CancellationToken.None);

        while (await reader.ReadAsync())
        {
            OperationType type = Enum.Parse<OperationType>(reader.GetString(reader.GetOrdinal("type")));
            OperationPayload payload = reader.GetJsonFieldValue<OperationPayload>(reader.GetOrdinal("payload"), _serializer);

            yield return new Operation(
                reader.GetString(reader.GetOrdinal("account_number")),
                type,
                reader.GetDecimal(reader.GetOrdinal("amount")),
                payload.InvoiceId,
                reader.GetDateTime(reader.GetOrdinal("timestamp")),
                reader.GetInt64(reader.GetOrdinal("id")));
        }
    }

    private sealed record OperationPayload(decimal Amount, Guid? InvoiceId);
}

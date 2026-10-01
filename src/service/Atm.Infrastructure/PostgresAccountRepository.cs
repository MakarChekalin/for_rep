using Atm.Domain;
using Itmo.Dev.Platform.Persistence.Abstractions.Commands;
using Itmo.Dev.Platform.Persistence.Abstractions.Connections;
using System.Data.Common;

namespace Atm.Infrastructure;

public class PostgresAccountRepository : IAccountRepository
{
    private readonly IPersistenceConnectionProvider _connectionProvider;

    public PostgresAccountRepository(IPersistenceConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<Account?> GetByNumberAsync(string number)
    {
        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(CancellationToken.None);

        await using IPersistenceCommand command = connection
            .CreateCommand("select number, pin_code, user_id, balance from accounts where number = @number")
            .AddParameter<string>("number", number);

        await using DbDataReader reader = await command.ExecuteReaderAsync(CancellationToken.None);

        if (!await reader.ReadAsync())
            return null;

        return new Account(
            reader.GetString(reader.GetOrdinal("number")),
            reader.GetString(reader.GetOrdinal("pin_code")),
            reader.GetString(reader.GetOrdinal("user_id")),
            reader.GetDecimal(reader.GetOrdinal("balance")));
    }

    public async Task SaveAsync(Account account)
    {
        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(CancellationToken.None);

        await using IPersistenceCommand command = connection
            .CreateCommand("""
                insert into accounts (number, pin_code, user_id, balance)
                values (@number, @pinCode, @userId, @balance)
                on conflict (number) do update set balance = @balance
                """)
            .AddParameter<string>("number", account.Number)
            .AddParameter<string>("pinCode", account.PinCode)
            .AddParameter<string>("userId", account.UserId)
            .AddParameter("balance", account.Balance);

        await command.ExecuteNonQueryAsync(CancellationToken.None);
    }

    public async Task<bool> ExistsAsync(string number)
    {
        Account? account = await GetByNumberAsync(number);
        return account != null;
    }

    public async Task<int> CountByUserIdAsync(string userId)
    {
        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(CancellationToken.None);

        await using IPersistenceCommand command = connection
            .CreateCommand("select count(*) from accounts where user_id = @userId")
            .AddParameter<string>("userId", userId);

        await using DbDataReader reader = await command.ExecuteReaderAsync(CancellationToken.None);

        await reader.ReadAsync();

        return checked((int)reader.GetInt64(0));
    }
}

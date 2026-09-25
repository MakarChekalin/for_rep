namespace Atm.Domain;

public interface IOperationRepository
{
    Task SaveAsync(Operation operation);

    IAsyncEnumerable<Operation> GetByAccountNumberAsync(string accountNumber);
}
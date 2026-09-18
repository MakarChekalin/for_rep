namespace Atm.Domain;

public interface IOperationRepository
{
    Task SaveAsync(Operation operation);
    Task<IReadOnlyList<Operation>> GetByAccountNumberAsync(string accountNumber);
}
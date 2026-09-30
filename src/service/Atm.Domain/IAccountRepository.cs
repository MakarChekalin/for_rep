namespace Atm.Domain;

public interface IAccountRepository // перешел на async(что бы пока шел запрос до дб программа не останавливалась)
{
    Task<Account?> GetByNumberAsync(string number);

    Task SaveAsync(Account account);

    Task<bool> ExistsAsync(string number); // сущ?
}
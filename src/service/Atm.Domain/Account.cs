namespace Atm.Domain;

public enum AccountType
{
    Personal,
    Corporate,
}

public class Account
{
    public string Number { get; } // get чтобы читать но не изменять(для защиты)

    public string PinCode { get; }

    public string UserId { get; }

    public AccountType Type { get; }

    public long ExternalId { get; }

    public decimal Balance { get; private set; } // тут уже можно изменять внутри класса

    public Account(string number, string pinCode, string userId, decimal balance, AccountType type = AccountType.Personal, long externalId = 0)
    {
        Number = number;
        PinCode = pinCode;
        UserId = userId;
        Balance = balance;
        Type = type;
        ExternalId = externalId;
    }

    public bool Withdraw(decimal amount) // метод списания
    {
        if (amount <= 0)
            return false;

        if (Balance < amount)
            return false;

        Balance -= amount;
        return true;
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
            return;

        Balance += amount;
    }
}

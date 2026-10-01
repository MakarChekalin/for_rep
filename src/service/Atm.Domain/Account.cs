namespace Atm.Domain;

public class Account
{
    public string Number { get; } // get чтобы читать но не изменять(для защиты)

    public string PinCode { get; }

    public string UserId { get; }

    public decimal Balance { get; private set; } // тут уже можно изменять внутри класса

    public Account(string number, string pinCode, string userId, decimal balance)
    {
        Number = number;
        PinCode = pinCode;
        UserId = userId;
        Balance = balance;
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
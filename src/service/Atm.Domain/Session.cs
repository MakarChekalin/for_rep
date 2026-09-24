namespace Atm.Domain;

public enum SessionType // enum - только 1 из
{
    User,
    Admin,
}

public class Session
{
    public Guid Key { get; } // ключ ссесии(как и говориться в задании)

    public SessionType Type { get; }

    public string? AccountNumber { get; } // может быть Null т к есть админы

    public Session(SessionType type, string? accountNumber = null, Guid? key = null)
    {
        Key = key ?? Guid.NewGuid();
        Type = type;
        AccountNumber = accountNumber;
    }
}
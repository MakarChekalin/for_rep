namespace Atm.Domain;

public class User
{
    public string Id { get; }

    public DateTime CreatedAt { get; }

    public User(string id, DateTime createdAt)
    {
        Id = id;
        CreatedAt = createdAt;
    }
}

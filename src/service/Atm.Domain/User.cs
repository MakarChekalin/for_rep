namespace Atm.Domain;

public class User
{
    public string Id { get; }

    public long ExternalId { get; }

    public DateTime CreatedAt { get; }

    public User(string id, DateTime createdAt, long externalId = 0)
    {
        Id = id;
        CreatedAt = createdAt;
        ExternalId = externalId;
    }
}

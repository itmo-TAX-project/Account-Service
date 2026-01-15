namespace Application.Models;

public class Account
{
    public Account(string name, string phone, AccountRole role, bool isBanned, DateTimeOffset createdAt, long id)
    {
        Id = id;
        Name = name;
        Phone = phone;
        Role = role;
        IsBanned = isBanned;
        CreatedAt = createdAt;
    }

    public Account(string name, string phone, AccountRole role)
    {
        Name = name;
        Phone = phone;
        Role = role;
        IsBanned = false;
        CreatedAt = DateTimeOffset.Now;
    }

    public long? Id { get; private set; }

    public string Name { get; private set; }

    public string Phone { get; private set; }

    public AccountRole Role { get; private set; }

    public bool IsBanned { get; private set; }

    public DateTimeOffset CreatedAt { get; }
}
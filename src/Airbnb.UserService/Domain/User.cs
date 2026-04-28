namespace Airbnb.UserService.Domain;

public enum UserRole
{
    Guest,
    Host
}

public class User
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string HashedPassword { get; private set; } = default!;
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private User() { }

    public User(string fullName, string email, string hashedPassword, UserRole role)
    {
        Id = Guid.NewGuid();
        FullName = fullName;
        Email = email;
        HashedPassword = hashedPassword;
        Role = role;
        CreatedAt = DateTime.UtcNow;
    }
}

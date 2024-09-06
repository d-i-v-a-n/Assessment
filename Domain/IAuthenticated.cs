namespace Domain;

public interface IAuthenticated
{
    string Id { get; }
    string Email { get; }
    bool IsAuthenticated { get; }
    bool IsModerator { get; }
}

public class AuthenticatedUser : IAuthenticated
{
    public AuthenticatedUser(
        string id,
        string email,
        bool isAuthenticated,
        bool isModerator)
    {
        Id = id;
        Email = email;
        IsAuthenticated = isAuthenticated;
        IsModerator = isModerator;
    }
    public string Id {get;}

    public string Email { get; }

    public bool IsAuthenticated { get; }

    public bool IsModerator { get; }
}
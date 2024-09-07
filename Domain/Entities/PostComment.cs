namespace Domain.Entities;

public class PostComment
{
    public int Id { get; private set; }
    public DateTime CreatedOnUtc { get; private set; } = DateTime.UtcNow;
    public string UserId { get; private set; }
    public User User { get; private set; }
    public int PostId { get; private set; }
    public Post Post { get; private set; }
    public string Comment { get; private set; } = string.Empty;

    internal static PostComment Create(
        User user,
        Post post,
        string comment) =>
        new()
        {
            UserId = user.Id,
            PostId = post.Id,
            Comment = comment,
        };
}

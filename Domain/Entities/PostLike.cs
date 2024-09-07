namespace Domain.Entities;

public class PostLike
{
    public DateTime CreatedOnUtc { get; private set; } = DateTime.UtcNow;
    public string UserId { get; private set; }
    public User User { get; private set; }
    public int PostId { get; private set; }
    public Post Post { get; private set; }

    internal static PostLike Create(
        User user,
        Post post
        ) =>
        new()
        {
            UserId = user.Id,
            PostId = post.Id
        };
}

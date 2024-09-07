namespace Domain.Entities;

public class PostTag
{
    public int Id { get; private set; }
    public DateTime CreatedOnUtc { get; private set; } = DateTime.UtcNow;
    public string UserId { get; private set; }
    public User User { get; private set; }
    public int PostId { get; private set; }
    public Post Post { get; private set; }
    public string Tag { get; private set; } = "misleading or false information";

    internal static PostTag Create(
        User user,
        Post post,
        string tag
        ) =>
        new()
        {
            UserId = user.Id,
            PostId = post.Id,
            Tag = tag,
        };
}
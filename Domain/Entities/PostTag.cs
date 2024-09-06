namespace Domain.Entities;

public class PostTag
{
    public int Id { get; internal set; }
    public DateTimeOffset CreatedOn { get; internal set; } = DateTimeOffset.UtcNow;
    public string UserId { get; internal set; }
    public User User { get; internal set; }
    public int PostId { get; internal set; }
    public Post Post { get; internal set; }
    public string Tag { get; internal set; } = "misleading or false information";

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
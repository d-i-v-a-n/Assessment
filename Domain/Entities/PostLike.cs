namespace Domain.Entities;

public class PostLike
{
    public DateTimeOffset CreatedOn { get; internal set; } = DateTimeOffset.UtcNow;
    public string UserId { get; internal set; }
    public User User { get; internal set; }
    public int PostId { get; internal set; }
    public Post Post { get; internal set; }

}

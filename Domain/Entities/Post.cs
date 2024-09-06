namespace Domain.Entities;

using Microsoft.AspNetCore.Identity;


//[EntityTypeConfiguration(typeof(PostConfiguration))]
public class Post
{
    public int Id { get; private set; }
    public DateTimeOffset CreatedOn { get; private set; } = DateTimeOffset.UtcNow;
    public string Content { get; private set; } = string.Empty;
    public int LikeCount { get; private set; } = 0;

    public string UserId { get; private set; } = string.Empty;
    public virtual IdentityUser User { get; set; }

    private readonly List<UserPostLike> _likes = [];
    public virtual IReadOnlyCollection<UserPostLike> Likes => _likes;
    private readonly List<UserPostComment> _comments = [];
    public virtual IReadOnlyCollection<UserPostComment> Comments => _comments;
    private readonly List<UserPostTag> _tags = [];
    public virtual IReadOnlyCollection<UserPostTag> Tags => _tags;

    //private Post() { }

    public static Post Create(
        string content,
        IdentityUser createdBy) =>
        new()
        {
            Content = content,
            UserId = createdBy.Id,
        };

    public void AddLike(
        IdentityUser likedBy)
    {
        if (UserId == likedBy.Id) return; // cannot like own post
        if (_likes.Exists(_ => _.UserId == likedBy.Id)) return; // cannot like same post more than once

        _likes.Add(new UserPostLike() { PostId = Id, UserId = likedBy.Id });
        LikeCount = _likes.Count;
    }

    public void RemoveLike(
        IdentityUser likedBy)
    {
        var like = _likes.Find(l => l.UserId == likedBy.Id);
        if (like is null) return;

        _likes.Remove(like);
        LikeCount = _likes.Count;
    }

    public void AddComment(
        string comment,
        IdentityUser commentBy)
    {
        _comments.Add(new() { PostId = Id, UserId = commentBy.Id, Comment = comment });
    }

    public void AddTag(
        IdentityUser user,
        Post post,
        string tag)
    {
        _tags.Add(UserPostTag.Create(user, post, tag));
    }
}


public class UserPostLike
{
    public DateTimeOffset CreatedOn { get; internal set; } = DateTimeOffset.UtcNow;
    public string UserId { get; internal set; }
    public IdentityUser User { get; internal set; }
    public int PostId { get; internal set; }
    public Post Post { get; internal set; }

}

public class UserPostComment
{
    public DateTimeOffset CreatedOn { get; internal set; } = DateTimeOffset.UtcNow;
    public string UserId { get; internal set; }
    public IdentityUser User { get; internal set; }
    public int PostId { get; internal set; }
    public Post Post { get; internal set; }
    public string Comment { get; internal set; } = string.Empty;
}

public class UserPostTag
{
    public DateTimeOffset CreatedOn { get; internal set; } = DateTimeOffset.UtcNow;
    public string UserId { get; internal set; }
    public IdentityUser User { get; internal set; }
    public int PostId { get; internal set; }
    public Post Post { get; internal set; }
    public string Tag { get; internal set; } = "misleading or false information";

    internal static UserPostTag Create(
        IdentityUser user,
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
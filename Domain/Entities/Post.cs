namespace Domain.Entities;

//[EntityTypeConfiguration(typeof(PostConfiguration))]
public class Post
{
    public int Id { get; private set; }
    public DateTime CreatedOnUtc { get; private set; } = DateTime.UtcNow;
    public string Content { get; private set; } = string.Empty;
    public int LikeCount { get; private set; } = 0;

    public string UserId { get; private set; } = string.Empty;
    public virtual User User { get; set; }

    private readonly List<PostLike> _likes = [];
    public virtual IReadOnlyCollection<PostLike> Likes => _likes;
    private readonly List<PostComment> _comments = [];
    public virtual IReadOnlyCollection<PostComment> Comments => _comments;
    private readonly List<PostTag> _tags = [];
    public virtual IReadOnlyCollection<PostTag> Tags => _tags;

    //private Post() { }

    public static Post Create(
        string content,
        User by) =>
        new()
        {
            Content = content,
            UserId = by.Id,
        };

    public void AddLike(
        User by)
    {
        if (UserId == by.Id) return; // cannot like own post
        if (_likes.Exists(_ => _.UserId == by.Id)) return; // cannot like same post more than once

        //_likes.Add(new PostLike() { PostId = Id, UserId = by.Id });
        _likes.Add(PostLike.Create(by, this));
        LikeCount = _likes.Count;
    }

    //public void RemoveLike(
    //    User by)
    //{
    //    var like = _likes.Find(l => l.UserId == by.Id);
    //    if (like is null) return;

    //    _likes.Remove(like);
    //    LikeCount = _likes.Count;
    //}

    public PostLike? FindLike(string userId) =>
        Likes.FirstOrDefault(l => l.UserId == userId);

    public void RemoveLike(
        PostLike like)
    {
        _likes.Remove(like);
        LikeCount = _likes.Count;
    }

    public void AddComment(
        User by,
        string comment)
    {
        //_comments.Add(new() { PostId = Id, UserId = by.Id, Comment = comment });
        _comments.Add(PostComment.Create(by, this, comment));
    }

    public void AddTag(
        User by,
        Post post,
        string tag)
    {
        _tags.Add(PostTag.Create(by, post, tag));
    }

    public PostLike? GetUserLike(User by) =>
        _likes.Find(l => l.UserId == by.Id);
}

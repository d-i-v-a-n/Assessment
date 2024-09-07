using System.Net;

namespace Application;

public class PagedPostsDto
{
    public List<PostDto> Posts { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    public PagedPostsDto(List<PostDto> posts, int totalCount, int pageNumber, int pageSize)
    {
        Posts = posts;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}

public class PostDto
{
    public int Id { get; set; }
    public string Content { get; set; } = "";
    public string Author { get; set; } = "";
    public DateTime CreatedOnUtc { get; set; }
    public int LikeCount { get; set; }
    public List<CommentDto> Comments { get; set; } = [];
    public List<TagDto> Tags { get; set; } = [];
}

public class CommentDto
{
    public int Id { get; set; }
    public string Content { get; set; } = "";
    public string Author { get; set; } = "unknown";
    public DateTime CreatedOnUtc { get; set; }
}

public class TagDto
{
    public string Tag { get; set; } = "";
}
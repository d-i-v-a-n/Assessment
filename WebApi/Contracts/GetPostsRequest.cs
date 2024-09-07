namespace WebApi.Contracts;

//public class LikePostRequest
//{
//    public int PostId { get; set; }
//}
public class GetPostsRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? AuthorEmail { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public string[]? Tags { get; set; }
    public string? SortBy { get; set; } = "date";
}
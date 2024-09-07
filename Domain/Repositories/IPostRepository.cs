using Domain.Entities;

namespace Domain.Repositories;

public interface IPostRepository
{
    void Add(Post post);
    void Update(Post post);
    Task<List<Post>> GetPostsAsync(CancellationToken ct = default);
    Task<Post> GetPostByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Post>> GetPagedPostsAsync(
        int pageNumber,
        int pageSize,
        string? authorEmail,
        DateTime? startDate,
        DateTime? endDate,
        string[]? tags,
        string? sortBy);
}

using Domain.Entities;

namespace Domain.Repositories;

public interface IPostRepository
{
    void Add(Post post);
    void Update(Post post);
    Task<List<Post>> GetPostsAsync(CancellationToken ct = default);
    Task<Post> GetPostByIdAsync(int id, CancellationToken ct = default);
}

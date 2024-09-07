using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public sealed class PostRepository : IPostRepository
{
    private readonly ApplicationDbContext _context;

    public PostRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(Post post) => _context.Add(post);

    public void Update(Post post) => _context.Update(post);
    
    public async Task<Post> GetPostByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Set<Post>()
            .Include(_ => _.User)
            .Include(_ => _.Likes)
            .FirstOrDefaultAsync(post => post.Id == id, ct);
    }

    public async Task<List<Post>> GetPostsAsync(CancellationToken ct = default)
    {
        return await _context.Set<Post>()
            .Include(_ => _.User)
            .Include(_ => _.Likes)
            .ToListAsync(cancellationToken: ct);
    }

    public async Task<(IEnumerable<Post> Posts, int TotalCount)> GetPagedPostsAsync(
    int pageNumber,
    int pageSize,
    string? authorEmail,
    DateTime? startDate,
    DateTime? endDate,
    string[]? tags,
    string? sortBy)
    {
        var query = _context.Set<Post>()
            .AsNoTracking()
            .Include(p => p.Comments).ThenInclude(c => c.User)
            .Include(p => p.User)
            .Include(p => p.Tags)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(authorEmail))
            query = query.Where(p => p.User.Email == authorEmail);

        if (startDate.HasValue)
            query = query.Where(p => p.CreatedOnUtc >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(p => p.CreatedOnUtc <= endDate.Value);

        if (tags != null && tags.Any())
            query = query.Where(p => p.Tags.Any(t => tags.Contains(t.Tag)));

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply sorting
        query = sortBy == "likes"
            ? query.OrderByDescending(p => p.LikeCount)
            : query.OrderByDescending(p => p.CreatedOnUtc);

        // Apply pagination
        var pagedPosts = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (pagedPosts, totalCount);
    }
}

public sealed class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<User> GetUserByEmailAsync(string email, CancellationToken ct = default)
    {
        return _context.Set<User>().FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    public Task<User> GetUserByIdAsync(string id, CancellationToken ct = default)
    {
        return _context.Set<User>().FirstOrDefaultAsync(u => u.Id == id, ct);
    }
}
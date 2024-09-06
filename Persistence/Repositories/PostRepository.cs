using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
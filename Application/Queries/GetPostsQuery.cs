using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using MediatR;

namespace Application.Queries;

public record GetPostsQuery() : IRequest<List<Post>>;


public sealed class GetPostsQueryHandler : IRequestHandler<GetPostsQuery, List<Post>>
{
    private readonly IPostRepository _repository;

    public GetPostsQueryHandler(IPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Post>> Handle(GetPostsQuery request, CancellationToken cancellationToken)
    {
        var posts = await _repository.GetPostsAsync(cancellationToken).ConfigureAwait(false);

        return posts;
    }
}



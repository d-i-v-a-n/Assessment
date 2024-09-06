using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using MediatR;

namespace Application.Queries;

public record GetPostQuery(int Id) : IRequest<Post>;

public sealed class GetPostQueryHandler : IRequestHandler<GetPostQuery, Post>
{
    private readonly IPostRepository _repository;

    public GetPostQueryHandler(IPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<Post> Handle(GetPostQuery request, CancellationToken cancellationToken)
    {
        var post = await _repository.GetPostByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);

        if (post is null)
            throw new CustomException("Post not found");

        return post;
    }
}
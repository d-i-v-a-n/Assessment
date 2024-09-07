using Domain.Repositories;
using MediatR;

namespace Application.Queries;

public record GetPostsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? AuthorEmail = null,
    DateTime? StartDateUtc = null,
    DateTime? EndDateUtc = null,
    string[]? Tags = null,
    string? SortBy = "date")
    : IRequest<PagedPostsDto>;


public sealed class GetPostsQueryHandler : IRequestHandler<GetPostsQuery, PagedPostsDto>
{
    private readonly IPostRepository _repository;

    public GetPostsQueryHandler(IPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedPostsDto> Handle(GetPostsQuery request, CancellationToken cancellationToken)
    {
        var posts = await _repository.GetPagedPostsAsync(
            request.PageNumber,
            request.PageSize,
            request.AuthorEmail,
            request.StartDateUtc,
            request.EndDateUtc,
            request.Tags,
            request.SortBy
        );

        var postDtos = posts.Select(post => new PostDto
        {
            Id = post.Id,
            Content = post.Content,
            Author = post.User.Email ?? "unknown",
            CreatedOnUtc = post.CreatedOnUtc,
            LikeCount = post.LikeCount,
            Comments = post.Comments.Select(comment => new CommentDto
            {
                Id = comment.Id,
                Content = comment.Comment,
                Author = comment.User.Email ?? "unknown",
                CreatedOnUtc = comment.CreatedOnUtc
            }).ToList(),
            Tags = post.Tags.Select(t => new TagDto()
            {
                Tag = t.Tag
            }).ToList()
        }).ToList();

        return new PagedPostsDto(postDtos, posts.Count(), request.PageNumber, request.PageSize);
    }
}
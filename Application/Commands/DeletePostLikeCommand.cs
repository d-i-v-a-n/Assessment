using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Domain;
using MediatR;

namespace Application.Commands;

public record DeletePostLikeCommand(int PostId) : IRequest;

public sealed class DeletePostLikeCommandHandler : IRequestHandler<DeletePostLikeCommand>
{
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthenticated _authenticated;

    public DeletePostLikeCommandHandler(IPostRepository postRepository, IUnitOfWork unitOfWork, IAuthenticated authenticated)
    {
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
        _authenticated = authenticated;
    }

    public async Task Handle(DeletePostLikeCommand request, CancellationToken cancellationToken)
    {
        (Post post, PostLike like) = await Validate(request, cancellationToken);

        post.RemoveLike(like);
        _postRepository.Update(post);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<(Post Post, PostLike Like)> Validate(DeletePostLikeCommand request, CancellationToken cancellationToken)
    {

        // find the post
        var post = await _postRepository.GetPostByIdAsync(request.PostId, cancellationToken);
        // TODO: make use of validation package?
        if (post is null)
            throw new CustomException($"Post with Id {request.PostId} not found");

        // find the like for this user
        var like = post.FindLike(_authenticated.Id);

        if (like is null)
            throw new CustomException($"Cannot find a like on the post for the user");

        return (post, like);
    }
}
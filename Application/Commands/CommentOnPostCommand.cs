using Domain.Repositories;
using Domain;
using MediatR;
using Domain.Exceptions;
using Domain.Entities;

namespace Application.Commands;

public record CommentOnPostCommand(int PostId, string Comment) : IRequest;

public sealed class CommentOnPostCommandHandler : IRequestHandler<CommentOnPostCommand>
{
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthenticated _authenticated;

    public CommentOnPostCommandHandler(IPostRepository postRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, IAuthenticated authenticated)
    {
        _postRepository = postRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _authenticated = authenticated;
    }

    public async Task Handle(CommentOnPostCommand request, CancellationToken cancellationToken)
    {
        (Post post, User user) = await Validate(request, cancellationToken);

        post.AddComment(user, request.Comment);
        _postRepository.Update(post);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<(Post Post, User User)> Validate(CommentOnPostCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetPostByIdAsync(request.PostId, cancellationToken);
        
        if (post is null) 
            throw new CustomException($"Post with Id {request.PostId} not found");
        
        var user = await _userRepository.GetUserByIdAsync(id: _authenticated.Id, ct: cancellationToken);

        return (post, user);
    }
}


using Domain.Repositories;
using Domain;
using MediatR;
using Domain.Exceptions;
using Domain.Entities;

namespace Application.Commands;

public record LikePostCommand(int PostId) : IRequest;

public sealed class LikePostCommandHandler : IRequestHandler<LikePostCommand>
{
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthenticated _authenticated;

    public LikePostCommandHandler(IPostRepository postRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, IAuthenticated authenticated)
    {
        _postRepository = postRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _authenticated = authenticated;
    }

    public async Task Handle(LikePostCommand request, CancellationToken cancellationToken)
    {
        (Post post, User user) = await Validate(request, cancellationToken);

        post.AddLike(user);
        _postRepository.Update(post);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<(Post Post, User User)> Validate(LikePostCommand request, CancellationToken cancellationToken)
    {

        // find the post
        var post = await _postRepository.GetPostByIdAsync(request.PostId, cancellationToken);
        // TODO: make use of validation package?
        if (post is null) 
            throw new CustomException($"Post with Id {request.PostId} not found");
        
        if(post.UserId == _authenticated.Id)
            throw new CustomException($"Cannot like own post");

        var user = await _userRepository.GetUserByIdAsync(id: _authenticated.Id, ct: cancellationToken);

        if (post.Likes.Any(_ => _.UserId == user.Id))
            throw new CustomException("Post already liked by user");
        
        return (post, user);
    }
}


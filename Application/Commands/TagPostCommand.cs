using Domain.Repositories;
using Domain;
using MediatR;
using Domain.Exceptions;
using Domain.Entities;

namespace Application.Commands;

public record TagPostCommand(int PostId, string Tag) : IRequest;

public sealed class TagPostCommandHandler : IRequestHandler<TagPostCommand>
{
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthenticated _authenticated;

    public TagPostCommandHandler(IPostRepository postRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, IAuthenticated authenticated)
    {
        _postRepository = postRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _authenticated = authenticated;
    }

    public async Task Handle(TagPostCommand request, CancellationToken cancellationToken)
    {
        (Post post, User user) = await Validate(request, cancellationToken);

        post.AddTag(user, post, request.Tag);
        _postRepository.Update(post);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<(Post Post, User User)> Validate(TagPostCommand request, CancellationToken cancellationToken)
    {

        // find the post
        var post = await _postRepository.GetPostByIdAsync(request.PostId, cancellationToken);
        // TODO: make use of validation package?
        if (post is null) 
            throw new CustomException($"Post with Id {request.PostId} not found");
        
        // todo: check user is a moderator

        if(post.UserId == _authenticated.Id)
            throw new CustomException($"Cannot Tag own post");

        var user = await _userRepository.GetUserByIdAsync(id: _authenticated.Id, ct: cancellationToken);

        //if (post.Tags.Any(_ => _.UserId == user.Id))
        //    throw new CustomException("Post already tagged");
        
        return (post, user);
    }
}


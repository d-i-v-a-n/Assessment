using Domain;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Commands;

public record AddPostCommand(string Content) : IRequest<int>;

public sealed class AddPostCommandHandler : IRequestHandler<AddPostCommand, int>
{
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthenticated _authenticated;

    public AddPostCommandHandler(IPostRepository postRepository, IUnitOfWork unitOfWork, IUserRepository userRepository, IAuthenticated authenticated)
    {
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _authenticated = authenticated;
    }

    public async Task<int> Handle(AddPostCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(id: _authenticated.Id, ct: cancellationToken);

        var post = Post.Create(content: request.Content, by: user);

        _postRepository.Add(post);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return post.Id;
    }
}

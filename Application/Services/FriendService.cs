using Application.Common.Exceptions;
using Application.DTOs.Friend;
using Application.Mappers;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;

namespace Application.Services;

public class FriendService(
    IFriendRepository friendRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<Guid> GetUserByNameAsync(string userName, CancellationToken ct = default)
    {
        var user = await userRepository.GetByUsernameAsync(userName, ct);
        if (user is null)
            throw new NotFoundException(nameof(User), userName);
        return user.Id;
    }
    
    public async Task<IEnumerable<FriendRequestDto>> GetIncomingRequestsAsync(Guid userId, CancellationToken ct = default)
    {
        var requests = await friendRepository.GetIncomingRequestsAsync(userId, ct);
        var result = new List<FriendRequestDto>();

        foreach (var request in requests)
        {
            var sender = await userRepository.GetByIdAsync(request.SenderId, ct);
            var receiver = await userRepository.GetByIdAsync(request.ReceiverId, ct);

            result.Add(new FriendRequestDto(
                request.Id,
                request.SenderId,
                sender?.Username ?? "Unknown",
                request.ReceiverId,
                receiver?.Username ?? "Unknown",
                request.Status.ToString(),
                request.CreatedAt
            ));
        }

        return result;
    }

    
    public async Task<FriendRequestDto> SendRequestAsync(
        Guid senderId,
        Guid receiverId,
        CancellationToken ct = default)
    {
        Console.WriteLine("send request");
        var sender = await userRepository.GetByIdAsync(senderId, ct)
            ?? throw new NotFoundException(nameof(User), senderId);

        var receiver = await userRepository.GetByIdAsync(receiverId, ct)
            ?? throw new NotFoundException(nameof(User), receiverId);
        
        var existing = await friendRepository.GetPendingRequestAsync(senderId, receiverId, ct);
        if (existing is not null)
            throw new ConflictException("Friend request already sent.");

        var request = FriendRequest.Send(senderId, receiverId);
        await friendRepository.AddRequestAsync(request, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return FriendMapper.ToDto(request, sender.Username, receiver.Username);
    }

    public async Task AcceptRequestAsync(Guid receiverId, Guid requestId, CancellationToken ct = default)
    {
        var request = await friendRepository.GetRequestByIdAsync(requestId, ct)
            ?? throw new NotFoundException(nameof(FriendRequest), requestId);

        if (request.ReceiverId != receiverId)
            throw new ForbiddenException("Cannot accept someone else's friend request.");

        request.Accept();

        var friendship = Friendship.Create(request.SenderId, request.ReceiverId);
        await friendRepository.AddFriendshipAsync(friendship, ct);
        friendRepository.UpdateRequest(request);
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeclineRequestAsync(Guid receiverId, Guid requestId, CancellationToken ct = default)
    {
        var request = await friendRepository.GetRequestByIdAsync(requestId, ct)
            ?? throw new NotFoundException(nameof(FriendRequest), requestId);

        if (request.ReceiverId != receiverId)
            throw new ForbiddenException("Cannot decline someone else's friend request.");

        request.Decline();
        friendRepository.UpdateRequest(request);
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<FriendshipDto>> GetFriendsAsync(Guid userId, CancellationToken ct = default)
    {
        var friendships = await friendRepository.GetFriendsByUserIdAsync(userId, ct);
        var result = new List<FriendshipDto>();

        foreach (var f in friendships)
        {
            var friendId = f.UserId1 == userId ? f.UserId2 : f.UserId1;
            var friend   = await userRepository.GetByIdAsync(friendId, ct);
            if (friend is not null)
                result.Add(new FriendshipDto(friend.Id, friend.Username));
            
        }

        return result;
    }
    
    public async Task RemoveFriendAsync(Guid userId, Guid friendUserId, CancellationToken ct = default)
    {
        var friendship = await friendRepository.GetFriendshipAsync(userId, friendUserId, ct)
                         ?? throw new NotFoundException(nameof(Friendship), friendUserId);

        friendRepository.RemoveFriendship(friendship);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
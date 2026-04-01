using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IFriendRepository
{
    Task<FriendRequest?> GetRequestByIdAsync(Guid id, CancellationToken ct = default);
    Task<FriendRequest?> GetPendingRequestAsync(Guid senderId, Guid receiverId, CancellationToken ct = default);
    Task<IEnumerable<Friendship>> GetFriendsByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<IEnumerable<FriendRequest>> GetIncomingRequestsAsync(Guid userId, CancellationToken ct = default);
    Task<IEnumerable<FriendRequest>> GetOutgoingRequestsAsync(Guid userId, CancellationToken ct = default);
    Task AddRequestAsync(FriendRequest request, CancellationToken ct = default);
    Task AddFriendshipAsync(Friendship friendship, CancellationToken ct = default);
    void UpdateRequest(FriendRequest request);
    
    Task<Friendship?> GetFriendshipAsync(Guid userId1, Guid userId2, CancellationToken ct = default);
    void RemoveFriendship(Friendship friendship);
}
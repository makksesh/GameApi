using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class FriendRepository(GameRpgDbContext context) : IFriendRepository
{
    public async Task<FriendRequest?> GetRequestByIdAsync(Guid id, CancellationToken ct = default)
        => await context.FriendRequests.FirstOrDefaultAsync(fr => fr.Id == id, ct);

    public async Task<FriendRequest?> GetPendingRequestAsync(
        Guid senderId, Guid receiverId, CancellationToken ct = default)
        => await context.FriendRequests
            .FirstOrDefaultAsync(fr =>
                fr.SenderId == senderId &&
                fr.ReceiverId == receiverId &&
                fr.Status == FriendRequestStatus.Pending, ct);

    public async Task<IEnumerable<Friendship>> GetFriendsByUserIdAsync(
        Guid userId, CancellationToken ct = default)
        => await context.Friendships
            .Where(f => f.UserId1 == userId || f.UserId2 == userId)
            .ToListAsync(ct);

    public async Task<IEnumerable<FriendRequest>> GetIncomingRequestsAsync(Guid userId, CancellationToken ct = default)
        => await context.FriendRequests
            .Where(fr => fr.ReceiverId == userId && fr.Status == FriendRequestStatus.Pending)
            .ToListAsync(ct);

    public async Task<IEnumerable<FriendRequest>> GetOutgoingRequestsAsync(Guid userId, CancellationToken ct = default)
        => await context.FriendRequests
            .Where(fr => fr.SenderId == userId && fr.Status == FriendRequestStatus.Pending)
            .ToListAsync(ct);

    
    public async Task AddRequestAsync(FriendRequest request, CancellationToken ct = default)
        => await context.FriendRequests.AddAsync(request, ct);

    public async Task AddFriendshipAsync(Friendship friendship, CancellationToken ct = default)
        => await context.Friendships.AddAsync(friendship, ct);

    public void UpdateRequest(FriendRequest request)
        => context.FriendRequests.Update(request);
    
    public async Task<Friendship?> GetFriendshipAsync(Guid userId1, Guid userId2, CancellationToken ct = default)
        => await context.Friendships
            .FirstOrDefaultAsync(f =>
                (f.UserId1 == userId1 && f.UserId2 == userId2) ||
                (f.UserId1 == userId2 && f.UserId2 == userId1), ct);
    
    public async Task<Friendship?> GetByPairAsync(Guid a, Guid b, CancellationToken ct)
    {
        return await context.Friendships
            .FirstOrDefaultAsync(
                f => (f.UserId1 == a && f.UserId2 == b) ||
                     (f.UserId1 == b && f.UserId2 == a), ct);
    }


    public void RemoveFriendship(Friendship friendship)
        => context.Friendships.Remove(friendship);

}
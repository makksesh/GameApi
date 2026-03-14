using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class FriendRequest : BaseEntity
{
    public Guid SenderId { get; private set; }
    public Guid ReceiverId { get; private set; }
    public FriendRequestStatus Status { get; private set; } = FriendRequestStatus.Pending;

    private FriendRequest() { }

    public static FriendRequest Send(Guid senderId, Guid receiverId)
    {
        if (senderId == receiverId)
            throw new InvalidOperationException("Cannot send friend request to yourself.");
        return new() { SenderId = senderId, ReceiverId = receiverId };
    }

    public void Accept()
    {
        if (Status != FriendRequestStatus.Pending)
            throw new InvalidOperationException("Request is no longer pending.");
        Status = FriendRequestStatus.Accepted;
        SetUpdatedAt();
    }

    public void Decline()
    {
        if (Status != FriendRequestStatus.Pending)
            throw new InvalidOperationException("Request is no longer pending.");
        Status = FriendRequestStatus.Declined;
        SetUpdatedAt();
    }
}
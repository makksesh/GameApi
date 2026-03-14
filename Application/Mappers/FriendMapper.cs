using Application.DTOs.Friend;
using Domain.Entities;

namespace Application.Mappers;

public static class FriendMapper
{
    public static FriendRequestDto ToDto(FriendRequest request, string senderUsername, string receiverUsername)
        => new(
            Id:               request.Id,
            SenderId:         request.SenderId,
            SenderUsername:   senderUsername,
            ReceiverId:       request.ReceiverId,
            ReceiverUsername: receiverUsername,
            Status:           request.Status.ToString(),
            CreatedAt:        request.CreatedAt
        );
}

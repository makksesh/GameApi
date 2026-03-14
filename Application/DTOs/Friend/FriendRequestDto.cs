namespace Application.DTOs.Friend;

public record FriendRequestDto(
    Guid Id,
    Guid SenderId,
    string SenderUsername,
    Guid ReceiverId,
    string ReceiverUsername,
    string Status,
    DateTime CreatedAt
);
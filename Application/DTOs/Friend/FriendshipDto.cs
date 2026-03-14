namespace Application.DTOs.Friend;

public record FriendshipDto(
    Guid UserId,
    string Username
);
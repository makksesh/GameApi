
namespace Application.DTOs.User;

public record UserDto(
    Guid Id,
    string Username,
    string Email,
    string Role,
    bool IsBlocked,
    DateTime CreatedAt
);

public record CreateUserDto(
    string Username,
    string Email,
    string Password
);
using Application.DTOs.Auth;
using Application.DTOs.User;
using Domain.Entities;

namespace Application.Mappers;

public static class UserMapper
{
    public static AuthResponse ToAuthResponse(User user, string token)
        => new(
            UserId:   user.Id,
            Username: user.Username,
            Email:    user.Email.Value,
            Role:     user.Role.ToString(),
            Token:    token,
            IsBlocked: user.IsBlocked,
            BlockedUntil: user.BlockedUntil
        );

    public static UserDto ToDto(User user)
        => new(
            Id:          user.Id,
            Username:    user.Username,
            Email:       user.Email.Value,
            Role:        user.Role.ToString(),
            IsBlocked:   user.IsBlocked,
            CreatedAt:   user.CreatedAt
        );
}
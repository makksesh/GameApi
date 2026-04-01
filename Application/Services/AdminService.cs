using Application.Common.Exceptions;
using Application.DTOs.User;
using Application.Mappers;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;

namespace Application.Services;

public class AdminService(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken ct = default)
    {
        var users = await userRepository.GetAllAsync(ct);
        return users.Select(UserMapper.ToDto);
    }

    public async Task<UserDto> BlockUserAsync(Guid adminId, Guid targetUserId, CancellationToken ct = default)
    {
        var admin = await userRepository.GetByIdAsync(adminId, ct)
            ?? throw new NotFoundException(nameof(User), adminId);

        if (admin.Role != UserRole.Moderator)
            throw new ForbiddenException("Only admins can block users.");

        var target = await userRepository.GetByIdAsync(targetUserId, ct)
            ?? throw new NotFoundException(nameof(User), targetUserId);

        if (target.Role == UserRole.Moderator)
            throw new BusinessRuleException("Cannot block another admin.");

        target.Block();
        userRepository.Update(target);
        await unitOfWork.SaveChangesAsync(ct);

        return UserMapper.ToDto(target);
    }

    public async Task<UserDto> UnblockUserAsync(Guid adminId, Guid targetUserId, CancellationToken ct = default)
    {
        var admin = await userRepository.GetByIdAsync(adminId, ct)
            ?? throw new NotFoundException(nameof(User), adminId);

        if (admin.Role != UserRole.Moderator)
            throw new ForbiddenException("Only admins can unblock users.");

        var target = await userRepository.GetByIdAsync(targetUserId, ct)
            ?? throw new NotFoundException(nameof(User), targetUserId);

        target.Unblock();
        userRepository.Update(target);
        await unitOfWork.SaveChangesAsync(ct);

        return UserMapper.ToDto(target);
    }

    public async Task<UserDto> ChangeUserRoleAsync(
        Guid adminId,
        Guid targetUserId,
        UpdateRoleRequest request,
        CancellationToken ct = default)
    {
        var admin = await userRepository.GetByIdAsync(adminId, ct)
            ?? throw new NotFoundException(nameof(User), adminId);

        if (admin.Role != UserRole.Moderator)
            throw new ForbiddenException("Only admins can change roles.");

        if (!Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var newRole))
            throw new BusinessRuleException($"Unknown role: {request.Role}.");

        var target = await userRepository.GetByIdAsync(targetUserId, ct)
            ?? throw new NotFoundException(nameof(User), targetUserId);

        target.ChangeRole(newRole);
        userRepository.Update(target);
        await unitOfWork.SaveChangesAsync(ct);

        return UserMapper.ToDto(target);
    }
}
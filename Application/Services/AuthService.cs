using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.DTOs.Auth;
using Application.Mappers;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;

namespace Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator)
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        // Валидация паролей
        if (request.Password != request.ConfirmPassword)
            throw new BusinessRuleException("Passwords do not match.");

        // Проверка уникальности username
        var existingByUsername = await userRepository.GetByUsernameAsync(request.Username, ct);
        if (existingByUsername is not null)
            throw new ConflictException($"Username '{request.Username}' is already taken.");

        // Проверка уникальности email
        var existingByEmail = await userRepository.GetByEmailAsync(request.Email, ct);
        if (existingByEmail is not null)
            throw new ConflictException($"Email '{request.Email}' is already registered.");

        // Создание пользователя
        var email        = new Email(request.Email);
        var passwordHash = passwordHasher.Hash(request.Password);
        var user         = User.Create(request.Username, email, passwordHash, request.DateOfBirth);

        // Сохранение
        await userRepository.AddAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);

        // Генерация токена и ответ
        var token = jwtTokenGenerator.GenerateToken(user);
        return UserMapper.ToAuthResponse(user, token);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        // Поиск пользователя
        var user = await userRepository.GetByUsernameAsync(request.Username, ct)
            ?? throw new NotFoundException(nameof(User), request.Username);

        // Проверка блокировки
        if (user.IsBlocked)
            throw new ForbiddenException("Your account has been blocked.");

        // Проверка пароля
        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new BusinessRuleException("Invalid username or password.");

        // Генерация токена и ответ
        var token = jwtTokenGenerator.GenerateToken(user);
        return UserMapper.ToAuthResponse(user, token);
    }

    public async Task ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken ct = default)
    {
        var user = await userRepository.GetByIdAsync(userId, ct)
            ?? throw new NotFoundException(nameof(User), userId);

        if (!passwordHasher.Verify(currentPassword, user.PasswordHash))
            throw new BusinessRuleException("Current password is incorrect.");

        var newHash = passwordHasher.Hash(newPassword);
        user.UpdatePassword(newHash);

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
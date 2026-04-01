namespace ModeratorWeb.Models;

public record AuthResponse(
    Guid UserId, string Username, string Email,
    string Role, string Token);

public record LoginRequest(string Username, string Password);

public record UserDto(
    Guid Id, string Username, string Email,
    string Role, bool IsBlocked, DateTime CreatedAt);

public record CreateUserDto(
    string Username,
    string Email,
    string Password
);

public record SupportTicketDto(
    Guid Id, Guid AuthorId, string AuthorUsername,
    Guid? AssignedModeratorId, string Subject, string Message,
    string? Resolution, string Status,
    DateTime CreatedAt, DateTime? UpdatedAt,
    List<SupportMessageDto> Messages);

public record SupportMessageDto(
    Guid Id, Guid TicketId, Guid AuthorId, string AuthorUsername,
    bool IsFromModerator, string Text, DateTime CreatedAt);

public record SendMessageRequest(string Text);
public record ResolveTicketRequest(string Resolution);

public record SkillDto(
    Guid Id, string Name, string Description,
    string Type, int MaxLevel, decimal LevelUpCost);

public record CreateSkillRequest(
    string Name, string Description,
    string Type, int MaxLevel, decimal LevelUpCost);

public record ItemDto(
    Guid Id, string Name, string Description,
    string Type, string Rarity, decimal BasePrice,
    int? BonusHealth, int? BonusMana,
    int? BonusArmor, int? BonusDamage);

public record CreateItemRequest(
    string Name, string Description,
    string Type, string Rarity, decimal BasePrice,
    int BonusHealth, int BonusMana,
    int BonusArmor, int BonusDamage);

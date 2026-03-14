namespace Application.DTOs.Support;

public record SupportTicketDto(
    Guid Id,
    Guid AuthorId,
    string AuthorUsername,
    Guid? AssignedModeratorId,
    string Subject,
    string Message,
    string? Resolution,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    List<SupportMessageDto> Messages
);
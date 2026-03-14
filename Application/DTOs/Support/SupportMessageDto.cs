namespace Application.DTOs.Support;

public record SupportMessageDto(
    Guid Id,
    Guid TicketId,
    Guid AuthorId,
    string AuthorUsername,
    bool IsFromModerator,
    string Text,
    DateTime CreatedAt
);
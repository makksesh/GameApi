using Application.DTOs.Support;
using Domain.Entities;

namespace Application.Mappers;

public static class SupportMessageMapper
{
    public static SupportMessageDto ToDto(SupportMessage m) => new(
        m.Id,
        m.TicketId,
        m.AuthorId,
        m.AuthorUsername,
        m.IsFromModerator,
        m.Text,
        m.CreatedAt
    );
}
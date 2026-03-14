using Application.DTOs.Support;
using Domain.Entities;

namespace Application.Mappers;

public static class SupportMapper
{
    public static SupportTicketDto ToDto(SupportTicket ticket, string authorUsername)
        => new(
            Id:                  ticket.Id,
            AuthorId:            ticket.AuthorId,
            AuthorUsername:      authorUsername,
            AssignedModeratorId: ticket.AssignedModeratorId,
            Subject:             ticket.Subject,
            Message:             ticket.Message,
            Resolution:          ticket.Resolution,
            Status:              ticket.Status.ToString(),
            CreatedAt:           ticket.CreatedAt,
            UpdatedAt:           ticket.UpdatedAt,
            Messages:            []
        );
    public static SupportTicketDto ToDto(
        SupportTicket ticket,
        string authorUsername,
        IEnumerable<SupportMessage> messages)
        => new(
            Id:                  ticket.Id,
            AuthorId:            ticket.AuthorId,
            AuthorUsername:      authorUsername,
            AssignedModeratorId: ticket.AssignedModeratorId,
            Subject:             ticket.Subject,
            Message:             ticket.Message,
            Resolution:          ticket.Resolution,
            Status:              ticket.Status.ToString(),
            CreatedAt:           ticket.CreatedAt,
            UpdatedAt:           ticket.UpdatedAt,
            Messages:            messages.Select(SupportMessageMapper.ToDto).ToList()
        );
}

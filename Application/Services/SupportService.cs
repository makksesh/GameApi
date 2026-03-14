using Application.Common.Exceptions;
using Application.DTOs.Support;
using Application.Mappers;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;

namespace Application.Services;

public class SupportService(
    ISupportRepository supportRepository,
    ISupportMessageRepository messageRepo,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<SupportTicketDto> CreateTicketAsync(
        Guid userId,
        CreateTicketRequest request,
        CancellationToken ct = default)
    {
        var user = await userRepository.GetByIdAsync(userId, ct)
            ?? throw new NotFoundException(nameof(User), userId);

        var ticket = SupportTicket.Create(userId, request.Subject, request.Message);
        await supportRepository.AddAsync(ticket, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return SupportMapper.ToDto(ticket, user.Username);
    }

    public async Task<IEnumerable<SupportTicketDto>> GetMyTicketsAsync(
        Guid userId,
        CancellationToken ct = default)
    {
        var user = await userRepository.GetByIdAsync(userId, ct)
            ?? throw new NotFoundException(nameof(User), userId);

        var tickets = await supportRepository.GetByAuthorIdAsync(userId, ct);
        return tickets.Select(t => SupportMapper.ToDto(t, user.Username));
    }


    public async Task<IEnumerable<SupportTicketDto>> GetTicketsByStatusAsync(
        SupportStatus status,
        CancellationToken ct = default)
    {
        var tickets = await supportRepository.GetByStatusAsync(status, ct);
        var result = new List<SupportTicketDto>();

        foreach (var ticket in tickets)
        {
            var author = await userRepository.GetByIdAsync(ticket.AuthorId, ct);
            result.Add(SupportMapper.ToDto(ticket, author?.Username ?? "Unknown"));
        }

        return result;
    }

    public async Task<SupportTicketDto> AssignModeratorAsync(
        Guid moderatorId,
        Guid ticketId,
        CancellationToken ct = default)
    {
        var moderator = await userRepository.GetByIdAsync(moderatorId, ct)
            ?? throw new NotFoundException(nameof(User), moderatorId);

        if (moderator.Role is not (UserRole.Moderator or UserRole.Admin))
            throw new ForbiddenException("Only moderators and admins can assign tickets.");

        var ticket = await supportRepository.GetByIdAsync(ticketId, ct)
            ?? throw new NotFoundException(nameof(SupportTicket), ticketId);

        ticket.AssignModerator(moderatorId);
        supportRepository.Update(ticket);
        await unitOfWork.SaveChangesAsync(ct);

        var author = await userRepository.GetByIdAsync(ticket.AuthorId, ct);
        return SupportMapper.ToDto(ticket, author?.Username ?? "Unknown");
    }

    public async Task<SupportTicketDto> ResolveTicketAsync(
        Guid moderatorId,
        Guid ticketId,
        ResolveTicketRequest request,
        CancellationToken ct = default)
    {
        var ticket = await supportRepository.GetByIdAsync(ticketId, ct)
            ?? throw new NotFoundException(nameof(SupportTicket), ticketId);

        if (ticket.AssignedModeratorId != moderatorId)
            throw new ForbiddenException("Only the assigned moderator can resolve this ticket.");

        ticket.Resolve(request.Resolution);
        supportRepository.Update(ticket);
        await unitOfWork.SaveChangesAsync(ct);

        var author = await userRepository.GetByIdAsync(ticket.AuthorId, ct);
        return SupportMapper.ToDto(ticket, author?.Username ?? "Unknown");
    }

    public async Task CloseTicketAsync(Guid userId, Guid ticketId, CancellationToken ct = default)
    {
        var ticket = await supportRepository.GetByIdAsync(ticketId, ct)
            ?? throw new NotFoundException(nameof(SupportTicket), ticketId);
        
        var user = await userRepository.GetByIdAsync(userId, ct)
            ?? throw new NotFoundException(nameof(User), userId);

        if (ticket.AuthorId != userId && user.Role != UserRole.Admin)
            throw new ForbiddenException("Only the author or admin can close a ticket.");

        ticket.Close();
        supportRepository.Update(ticket);
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task ReopenTicketAsync(Guid userId, Guid ticketId, CancellationToken ct = default)
    {
        var ticket = await supportRepository.GetByIdAsync(ticketId, ct)
            ?? throw new NotFoundException(nameof(SupportTicket), ticketId);

        if (ticket.AuthorId != userId)
            throw new ForbiddenException("Only the author can reopen a ticket.");

        ticket.Reopen();
        supportRepository.Update(ticket);
        await unitOfWork.SaveChangesAsync(ct);
    }
    
    public async Task<IEnumerable<SupportMessageDto>> GetMessagesAsync(
        Guid requesterId,
        bool isModerator,
        Guid ticketId,
        CancellationToken ct = default)
    {
        var ticket = await supportRepository.GetByIdAsync(ticketId, ct)
                     ?? throw new NotFoundException(nameof(SupportTicket), ticketId);
        
        if (!isModerator && ticket.AuthorId != requesterId)
            throw new ForbiddenException("Access denied.");

        var messages = await messageRepo.GetByTicketIdAsync(ticketId, ct);
        return messages.Select(SupportMessageMapper.ToDto);
    }

    public async Task<SupportMessageDto> SendMessageAsync(
        Guid authorId,
        bool isFromModerator,
        Guid ticketId,
        SendMessageRequest request,
        CancellationToken ct = default)
    {
        var ticket = await supportRepository.GetByIdAsync(ticketId, ct)
                     ?? throw new NotFoundException(nameof(SupportTicket), ticketId);
        
        if (ticket.Status is SupportStatus.Closed or SupportStatus.Resolved)
            throw new ForbiddenException("Cannot send messages to a closed ticket.");
        
        if (!isFromModerator && ticket.AuthorId != authorId)
            throw new ForbiddenException("Access denied.");

        var user = await userRepository.GetByIdAsync(authorId, ct)
                   ?? throw new NotFoundException(nameof(User), authorId);

        var message = SupportMessage.Create(ticketId, authorId, user.Username, isFromModerator, request.Text);
        await messageRepo.AddAsync(message, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return SupportMessageMapper.ToDto(message);
    }
}
using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class SupportTicket : BaseEntity, IAggregateRoot
{
    public Guid AuthorId { get; private set; }
    public Guid? AssignedModeratorId { get; private set; }
    public string Subject { get; private set; } = null!;
    public string Message { get; private set; } = null!;
    public string? Resolution { get; private set; }
    public SupportStatus Status { get; private set; } = SupportStatus.New;
    public ICollection<SupportMessage> Messages { get; private set; } = new List<SupportMessage>();

    private SupportTicket() { }

    public static SupportTicket Create(Guid authorId, string subject, string message)
    {
        if (string.IsNullOrWhiteSpace(subject)) throw new ArgumentException("Subject is required.");
        if (string.IsNullOrWhiteSpace(message)) throw new ArgumentException("Message is required.");
        return new() { AuthorId = authorId, Subject = subject, Message = message };
    }

    public void AssignModerator(Guid moderatorId)
    {
        if (Status != SupportStatus.New)
            throw new InvalidOperationException("Ticket is already being processed.");
        AssignedModeratorId = moderatorId;
        Status = SupportStatus.InProgress;
        SetUpdatedAt();
    }

    public void Resolve(string resolution)
    {
        if (Status != SupportStatus.InProgress)
            throw new InvalidOperationException("Ticket must be in progress to resolve.");
        Resolution = resolution;
        Status = SupportStatus.Resolved;
        SetUpdatedAt();
    }

    public void Close()
    {
        if (Status is SupportStatus.Closed)
            throw new InvalidOperationException("Ticket is already closed.");
        Status = SupportStatus.Closed;
        SetUpdatedAt();
    }

    public void Reopen()
    {
        if (Status != SupportStatus.Resolved)
            throw new InvalidOperationException("Only resolved tickets can be reopened.");
        Status = SupportStatus.New;           // Сбрасываем в New, чтобы можно было переназначить
        AssignedModeratorId = null;           
        Resolution = null;                    
        SetUpdatedAt();
    }

}
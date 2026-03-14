namespace Domain.Entities;

public class SupportMessage
{
    public Guid Id { get; private set; }
    public Guid TicketId { get; private set; }
    public Guid AuthorId { get; private set; }
    public string AuthorUsername { get; private set; } = string.Empty;
    public bool IsFromModerator { get; private set; }
    public string Text { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    private SupportMessage() { } 

    public static SupportMessage Create(
        Guid ticketId, Guid authorId, string authorUsername,
        bool isFromModerator, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Message text cannot be empty.");

        return new SupportMessage
        {
            Id = Guid.NewGuid(),
            TicketId = ticketId,
            AuthorId = authorId,
            AuthorUsername = authorUsername,
            IsFromModerator = isFromModerator,
            Text = text,
            CreatedAt = DateTime.UtcNow
        };
    }
}
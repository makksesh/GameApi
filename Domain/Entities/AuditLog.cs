using Domain.Common;

namespace Domain.Entities;

public class AuditLog : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Action { get; private set; } = null!;
    public string? Details { get; private set; }
    public string? IpAddress { get; private set; }

    private AuditLog() { }

    public static AuditLog Record(Guid userId, string action, string? details = null, string? ipAddress = null)
        => new() { UserId = userId, Action = action, Details = details, IpAddress = ipAddress };
}
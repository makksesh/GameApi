using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces.Repositories;

public interface ISupportRepository
{
    Task<SupportTicket?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<SupportTicket>> GetByStatusAsync(SupportStatus status, CancellationToken ct = default);
    Task<IEnumerable<SupportTicket>> GetByAuthorIdAsync(Guid authorId, CancellationToken ct = default);
    Task AddAsync(SupportTicket ticket, CancellationToken ct = default);
    void Update(SupportTicket ticket);
}
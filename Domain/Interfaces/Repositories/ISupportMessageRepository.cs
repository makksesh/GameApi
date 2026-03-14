using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface ISupportMessageRepository
{
    Task<IEnumerable<SupportMessage>> GetByTicketIdAsync(Guid ticketId, CancellationToken ct = default);
    Task AddAsync(SupportMessage message, CancellationToken ct = default);
}
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;


public class SupportMessageRepository : ISupportMessageRepository
{
    private readonly GameRpgDbContext _db;

    public SupportMessageRepository(GameRpgDbContext db) => _db = db;

    public async Task<IEnumerable<SupportMessage>> GetByTicketIdAsync(Guid ticketId, CancellationToken ct = default) =>
        await _db.SupportMessages
            .Where(m => m.TicketId == ticketId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync(ct);

    public async Task AddAsync(SupportMessage message, CancellationToken ct = default) =>
        await _db.SupportMessages.AddAsync(message, ct);
}
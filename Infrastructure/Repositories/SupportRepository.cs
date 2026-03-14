using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SupportRepository(GameRpgDbContext context) : ISupportRepository
{
    public async Task<SupportTicket?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.SupportTickets.FirstOrDefaultAsync(st => st.Id == id, ct);

    public async Task<IEnumerable<SupportTicket>> GetByStatusAsync(
        SupportStatus status, CancellationToken ct = default)
        => await context.SupportTickets
            .Where(st => st.Status == status)
            .ToListAsync(ct);

    public async Task<IEnumerable<SupportTicket>> GetByAuthorIdAsync(
        Guid authorId, CancellationToken ct = default)
        => await context.SupportTickets
            .Where(st => st.AuthorId == authorId)
            .ToListAsync(ct);

    public async Task AddAsync(SupportTicket ticket, CancellationToken ct = default)
        => await context.SupportTickets.AddAsync(ticket, ct);

    public void Update(SupportTicket ticket)
        => context.SupportTickets.Update(ticket);
}
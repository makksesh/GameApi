using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TradeRepository(GameRpgDbContext context) : ITradeRepository
{
    public async Task<TradeLot?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.TradeLots
            .Include(t => t.Item)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<IEnumerable<TradeLot>> GetActiveLotsAsync(CancellationToken ct = default)
        => await context.TradeLots
            .Include(t => t.Item)
            .Where(t => t.Status == TradeLotStatus.Active)
            .ToListAsync(ct);

    public async Task<IEnumerable<TradeLot>> GetBySellerIdAsync(Guid sellerId, CancellationToken ct = default)
        => await context.TradeLots
            .Include(t => t.Item)
            .Where(t => t.SellerId == sellerId)
            .ToListAsync(ct);

    public async Task AddAsync(TradeLot lot, CancellationToken ct = default)
        => await context.TradeLots.AddAsync(lot, ct);

    public void Update(TradeLot lot) => context.TradeLots.Update(lot);

    public async Task AddPurchaseAsync(Purchase purchase, CancellationToken ct = default)
        => await context.Purchases.AddAsync(purchase, ct);
}
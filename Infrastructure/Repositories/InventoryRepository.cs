using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class InventoryRepository(GameRpgDbContext context) : IInventoryRepository
{
    public async Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.InventoryItems
            .Include(ii => ii.Item)
            .FirstOrDefaultAsync(ii => ii.Id == id, ct);

    public async Task<InventoryItem?> GetByCharacterAndItemAsync(
        Guid characterId, Guid itemId, CancellationToken ct = default)
        => await context.InventoryItems
            .Include(ii => ii.Item)
            .FirstOrDefaultAsync(ii => ii.CharacterId == characterId && ii.ItemId == itemId, ct);

    public async Task<IEnumerable<InventoryItem>> GetByCharacterIdAsync(
        Guid characterId, CancellationToken ct = default)
        => await context.InventoryItems
            .Include(ii => ii.Item)
            .Where(ii => ii.CharacterId == characterId)
            .ToListAsync(ct);

    public async Task AddAsync(InventoryItem item, CancellationToken ct = default)
        => await context.InventoryItems.AddAsync(item, ct);

    public void Update(InventoryItem item) => context.InventoryItems.Update(item);
    public void Delete(InventoryItem item) => context.InventoryItems.Remove(item);
}
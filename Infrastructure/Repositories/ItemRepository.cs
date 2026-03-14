using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ItemRepository(GameRpgDbContext context) : IItemRepository
{
    public async Task<Item?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.Items.FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task<IEnumerable<Item>> GetAllAsync(CancellationToken ct = default)
        => await context.Items.ToListAsync(ct);

    public async Task<IEnumerable<Item>> GetByTypeAsync(ItemType type, CancellationToken ct = default)
        => await context.Items.Where(i => i.Type == type).ToListAsync(ct);

    public async Task<IEnumerable<Item>> GetByRarityAsync(ItemRarity rarity, CancellationToken ct = default)
        => await context.Items.Where(i => i.Rarity == rarity).ToListAsync(ct);

    public async Task<IEnumerable<Item>> SearchByNameAsync(string namePart, CancellationToken ct = default)
        => await context.Items
            .Where(i => EF.Functions.ILike(i.Name, $"%{namePart}%"))
            .ToListAsync(ct);

    public async Task AddAsync(Item item, CancellationToken ct = default)
        => await context.Items.AddAsync(item, ct);

    public void Update(Item item) => context.Items.Update(item);
    public void Delete(Item item) => context.Items.Remove(item);
}
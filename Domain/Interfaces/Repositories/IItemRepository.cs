using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces.Repositories;

public interface IItemRepository
{
    Task<Item?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IEnumerable<Item>> GetAllAsync(CancellationToken ct = default);

    Task<IEnumerable<Item>> GetByTypeAsync(ItemType type, CancellationToken ct = default);
    
    Task<IEnumerable<Item>> GetByRarityAsync(ItemRarity rarity, CancellationToken ct = default);
    
    Task<IEnumerable<Item>> SearchByNameAsync(string namePart, CancellationToken ct = default);

    Task AddAsync(Item item, CancellationToken ct = default);
    void Update(Item item);
    void Delete(Item item);
}
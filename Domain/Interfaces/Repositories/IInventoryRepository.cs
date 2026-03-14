using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IInventoryRepository
{
    Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<InventoryItem?> GetByCharacterAndItemAsync(Guid characterId, Guid itemId, CancellationToken ct = default);
    Task<IEnumerable<InventoryItem>> GetByCharacterIdAsync(Guid characterId, CancellationToken ct = default);
    Task AddAsync(InventoryItem item, CancellationToken ct = default);
    void Update(InventoryItem item);
    void Delete(InventoryItem item);
}
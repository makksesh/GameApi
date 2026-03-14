using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface ICharacterRepository
{
    Task<Character?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Character?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(Character character, CancellationToken ct = default);
    void Update(Character character);
}
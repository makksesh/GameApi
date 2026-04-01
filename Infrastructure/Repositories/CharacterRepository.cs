using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CharacterRepository(GameRpgDbContext context) : ICharacterRepository
{
    public async Task<Character?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.Characters
            .Include(c => c.Skills).ThenInclude(cs => cs.Skill)
            .Include(c => c.Inventory).ThenInclude(ii => ii.Item)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<Character?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        => await context.Characters
            .Include(c => c.Skills).ThenInclude(cs => cs.Skill)
            .Include(c => c.Inventory).ThenInclude(ii => ii.Item)
            .FirstOrDefaultAsync(c => c.UserId == userId, ct);

    public async Task AddAsync(Character character, CancellationToken ct = default)
        => await context.Characters.AddAsync(character, ct);

    public void Update(Character character)
    {
        // EF Core отслеживает сущность автоматически после GetByUserIdAsync.
        // Явный вызов не нужен, но оставляем для совместимости с интерфейсом.
        // context.Entry(character).State = EntityState.Modified; — не нужно для tracked entity
    }
}
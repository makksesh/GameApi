using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CharacterSkillRepository(GameRpgDbContext context) : ICharacterSkillRepository
{
    public async Task AddAsync(CharacterSkill characterSkill, CancellationToken ct = default)
        => await context.CharacterSkills.AddAsync(characterSkill, ct);

    public async Task<CharacterSkill?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.CharacterSkills
            .Include(cs => cs.Skill)
            .FirstOrDefaultAsync(cs => cs.Id == id, ct);
}
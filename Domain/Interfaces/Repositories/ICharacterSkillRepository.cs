using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface ICharacterSkillRepository
{
    Task AddAsync(CharacterSkill characterSkill, CancellationToken ct = default);
    Task<CharacterSkill?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
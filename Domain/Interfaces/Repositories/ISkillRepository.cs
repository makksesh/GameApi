using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces.Repositories;

public interface ISkillRepository
{
    Task<Skill?> GetByIdAsync(Guid id, CancellationToken ct = default);
    
    Task<IEnumerable<Skill>> GetAllAsync(CancellationToken ct = default);
    
    Task<IEnumerable<Skill>> GetByTypeAsync(SkillType type, CancellationToken ct = default);
    
    Task<IEnumerable<Skill>> SearchByNameAsync(string namePart, CancellationToken ct = default);

    Task AddAsync(Skill skill, CancellationToken ct = default);
    void Update(Skill skill);
    void Delete(Skill skill);
}
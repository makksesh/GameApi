using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SkillRepository(GameRpgDbContext context) : ISkillRepository
{
    public async Task<Skill?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.Skills.FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IEnumerable<Skill>> GetAllAsync(CancellationToken ct = default)
        => await context.Skills.ToListAsync(ct);

    public async Task<IEnumerable<Skill>> GetByTypeAsync(SkillType type, CancellationToken ct = default)
        => await context.Skills.Where(s => s.Type == type).ToListAsync(ct);

    public async Task<IEnumerable<Skill>> SearchByNameAsync(string namePart, CancellationToken ct = default)
        => await context.Skills
            .Where(s => EF.Functions.ILike(s.Name, $"%{namePart}%"))
            .ToListAsync(ct);

    public async Task AddAsync(Skill skill, CancellationToken ct = default)
        => await context.Skills.AddAsync(skill, ct);

    public void Update(Skill skill) => context.Skills.Update(skill);
    public void Delete(Skill skill) => context.Skills.Remove(skill);
}
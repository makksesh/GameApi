namespace Application.DTOs.Skill;

public record SkillDto(
    Guid Id,
    string Name,
    string Description,
    string Type,
    int MaxLevel,
    decimal LevelUpCost
);
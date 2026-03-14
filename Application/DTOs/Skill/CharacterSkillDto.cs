namespace Application.DTOs.Skill;

public record CharacterSkillDto(
    Guid Id,
    Guid SkillId,
    string SkillName,
    string Description,
    string Type,
    int CurrentLevel,
    int MaxLevel,
    decimal LevelUpCost
);
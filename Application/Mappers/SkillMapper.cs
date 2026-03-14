using Application.DTOs.Skill;
using Domain.Entities;

namespace Application.Mappers;

public static class SkillMapper
{
    public static SkillDto ToDto(Skill skill)
        => new(
            Id:          skill.Id,
            Name:        skill.Name,
            Description: skill.Description,
            Type:        skill.Type.ToString(),
            MaxLevel:    skill.MaxLevel,
            LevelUpCost: skill.LevelUpCost.Amount
        );

    public static CharacterSkillDto ToCharacterSkillDto(CharacterSkill cs)
        => new(
            Id:          cs.Id,
            SkillId:     cs.SkillId,
            SkillName:   cs.Skill.Name,
            Description: cs.Skill.Description,
            Type:        cs.Skill.Type.ToString(),
            CurrentLevel: cs.CurrentLevel,
            MaxLevel:    cs.Skill.MaxLevel,
            LevelUpCost: cs.Skill.LevelUpCost.Amount
        );
}

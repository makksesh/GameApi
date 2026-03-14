using Domain.Common;

namespace Domain.Entities;

/// <summary>Связь персонаж - навык, с текущим уровнем прокачки.</summary>
public class CharacterSkill : BaseEntity
{
    public Guid CharacterId { get; private set; }
    public Guid SkillId { get; private set; }
    public int CurrentLevel { get; private set; } = 1;

    public Skill Skill { get; private set; } = null!;

    private CharacterSkill() { }

    public static CharacterSkill Learn(Guid characterId, Guid skillId)
        => new() { CharacterId = characterId, SkillId = skillId };

    public void LevelUp()
    {
        if (CurrentLevel >= Skill.MaxLevel)
            throw new InvalidOperationException("Skill is already at max level.");
        CurrentLevel++;
        SetUpdatedAt();
    }
}
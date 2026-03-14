using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Skill : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public SkillType Type { get; private set; }
    public int MaxLevel { get; private set; }
    public Money LevelUpCost { get; private set; } = null!;

    private Skill() { }

    public static Skill Create(string name, string description, SkillType type, int maxLevel, Money levelUpCost)
        => new()
        {
            Name        = name,
            Description = description,
            Type        = type,
            MaxLevel    = maxLevel,
            LevelUpCost = levelUpCost
        };
}
using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Character : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = null!;
    public int Level { get; private set; } = 1;
    public int Experience { get; private set; }
    public StatBlock Stats { get; private set; } = null!;
    public Money Balance { get; private set; } = Money.Zero;

    public IReadOnlyCollection<CharacterSkill> Skills => _skills.AsReadOnly();
    public IReadOnlyCollection<InventoryItem> Inventory => _inventory.AsReadOnly();

    private readonly List<CharacterSkill> _skills = [];
    private readonly List<InventoryItem> _inventory = [];

    private Character() { }

    public static Character Create(Guid userId, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Character name cannot be empty.", nameof(name));

        return new Character
        {
            UserId = userId,
            Name   = name,
            Stats  = StatBlock.Default,
            Balance = new Money(200) 
        };
    }
    
    public CharacterSkill LearnSkill(Guid skillId)
    {
        var characterSkill = CharacterSkill.Learn(Id, skillId);
        _skills.Add(characterSkill);
        SetUpdatedAt();
        return characterSkill;
    }

    public void AddExperience(int amount)
    {
        if (amount <= 0) return;
        Experience += amount;
        
        while (Experience >= Level * 100)
        {
            Experience -= Level * 100;
            Level++;
            Stats = Stats.WithUpgrade(health: 10, mana: 5, armor: 2, damage: 3);
        }
        SetUpdatedAt();
    }

    public void UpgradeStat(string stat, int amount)
    {
        Stats = stat.ToLower() switch
        {
            "health" => Stats.WithUpgrade(health: amount),
            "mana"   => Stats.WithUpgrade(mana: amount),
            "armor"  => Stats.WithUpgrade(armor: amount),
            "damage" => Stats.WithUpgrade(damage: amount),
            _ => throw new ArgumentException($"Unknown stat: {stat}")
        };
        SetUpdatedAt();
    }

    public void Deposit(Money amount)
    {
        Balance = Balance.Add(amount);
        SetUpdatedAt();
    }

    public void Withdraw(Money amount)
    {
        Balance = Balance.Subtract(amount); // todo: обработать
        SetUpdatedAt();
    }
}
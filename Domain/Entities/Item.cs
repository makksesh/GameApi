using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Item : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public ItemType Type { get; private set; }
    public ItemRarity Rarity { get; private set; }
    public Money BasePrice { get; private set; } = null!;
    public StatBlock? BonusStats { get; private set; }

    private Item() { }

    public static Item Create(
        string name, string description,
        ItemType type, ItemRarity rarity,
        Money basePrice, StatBlock? bonusStats = null)
        => new()
        {
            Name        = name,
            Description = description,
            Type        = type,
            Rarity      = rarity,
            BasePrice   = basePrice,
            BonusStats  = bonusStats
        };
    public void Update(string name, string description,
        ItemType type, ItemRarity rarity,
        Money basePrice, StatBlock? bonusStats)
    {
        Name = name;
        Description = description;
        Type = type;
        Rarity = rarity;
        BasePrice = basePrice;
        BonusStats = bonusStats;
        SetUpdatedAt();
    }

}
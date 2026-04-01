namespace Application.DTOs.Item;

public record ItemDto(
    Guid Id, string Name, string Description,
    string Type, string Rarity, decimal BasePrice,
    int? BonusHealth, int? BonusMana,
    int? BonusArmor, int? BonusDamage);

public record CreateItemRequest(
    string Name, string Description,
    string Type, string Rarity, decimal BasePrice,
    int BonusHealth, int BonusMana,
    int BonusArmor, int BonusDamage);

public record UpdateItemRequest(
    string Name, string Description,
    string Type, string Rarity, decimal BasePrice,
    int BonusHealth, int BonusMana,
    int BonusArmor, int BonusDamage);
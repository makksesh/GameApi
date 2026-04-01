using Application.DTOs.Item;
using Domain.Entities;

namespace Application.Mappers;

public static class ItemMapper
{
    public static ItemDto ToDto(Item item) => new(
        item.Id, item.Name, item.Description,
        item.Type.ToString(), item.Rarity.ToString(),
        item.BasePrice.Amount,
        item.BonusStats?.Health, item.BonusStats?.Mana,
        item.BonusStats?.Armor, item.BonusStats?.Damage);
}
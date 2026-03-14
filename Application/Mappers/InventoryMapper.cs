using Application.DTOs.Inventory;
using Domain.Entities;

namespace Application.Mappers;

public static class InventoryMapper
{
    public static InventoryItemDto ToDto(InventoryItem item)
        => new(
            Id:        item.Id,
            ItemId:    item.ItemId,
            ItemName:  item.Item.Name,
            ItemType:  item.Item.Type.ToString(),
            Rarity:    item.Item.Rarity.ToString(),
            Quantity:  item.Quantity,
            IsEquipped: item.IsEquipped,
            BasePrice: item.Item.BasePrice.Amount
        );
}
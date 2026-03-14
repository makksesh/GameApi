namespace Application.DTOs.Inventory;

public record InventoryItemDto(
    Guid Id,
    Guid ItemId,
    string ItemName,
    string ItemType,
    string Rarity,
    int Quantity,
    bool IsEquipped,
    decimal BasePrice
);
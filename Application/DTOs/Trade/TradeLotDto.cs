namespace Application.DTOs.Trade;

public record TradeLotDto(
    Guid Id,
    Guid SellerId,
    Guid ItemId,
    string ItemName,
    string ItemRarity,
    int Quantity,
    decimal Price,
    string Status,
    DateTime CreatedAt
);
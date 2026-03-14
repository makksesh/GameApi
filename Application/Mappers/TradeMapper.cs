using Application.DTOs.Trade;
using Domain.Entities;

namespace Application.Mappers;

public static class TradeMapper
{
    public static TradeLotDto ToDto(TradeLot lot)
        => new(
            Id:         lot.Id,
            SellerId:   lot.SellerId,
            ItemId:     lot.ItemId,
            ItemName:   lot.Item.Name,
            ItemRarity: lot.Item.Rarity.ToString(),
            Quantity:   lot.Quantity,
            Price:      lot.Price.Amount,
            Status:     lot.Status.ToString(),
            CreatedAt:  lot.CreatedAt
        );
}
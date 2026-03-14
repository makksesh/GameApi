using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities;

public class TradeLot : BaseEntity, IAggregateRoot
{
    public Guid SellerId { get; private set; }
    public Guid ItemId { get; private set; }
    public int Quantity { get; private set; }
    public Money Price { get; private set; } = null!;
    public TradeLotStatus Status { get; private set; } = TradeLotStatus.Active;

    public Item Item { get; private set; } = null!;

    private TradeLot() { }

    public static TradeLot Create(Guid sellerId, Guid itemId, int quantity, Money price)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive.");
        return new() { SellerId = sellerId, ItemId = itemId, Quantity = quantity, Price = price };
    }

    public void Cancel()
    {
        if (Status != TradeLotStatus.Active)
            throw new InvalidOperationException("Only active lots can be cancelled.");
        Status = TradeLotStatus.Cancelled;
        SetUpdatedAt();
    }

    public void MarkAsSold()
    {
        if (Status != TradeLotStatus.Active)
            throw new InvalidOperationException("Only active lots can be sold.");
        Status = TradeLotStatus.Sold;
        SetUpdatedAt();
    }
}
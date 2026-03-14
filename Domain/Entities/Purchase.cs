using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Purchase : BaseEntity
{
    public Guid BuyerId { get; private set; }
    public Guid TradeLotId { get; private set; }
    public int Quantity { get; private set; }
    public Money TotalPrice { get; private set; } = null!;

    private Purchase() { }

    public static Purchase Create(Guid buyerId, Guid tradeLotId, int quantity, Money totalPrice)
        => new() { BuyerId = buyerId, TradeLotId = tradeLotId, Quantity = quantity, TotalPrice = totalPrice };
}
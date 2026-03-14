using Domain.Common;

namespace Domain.Entities;

public class InventoryItem : BaseEntity
{
    public Guid CharacterId { get; private set; }
    public Guid ItemId { get; private set; }
    public int Quantity { get; private set; }
    public bool IsEquipped { get; private set; }

    public Item Item { get; private set; } = null!;

    private InventoryItem() { }

    public static InventoryItem Create(Guid characterId, Guid itemId, int quantity = 1)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive.");
        return new() { CharacterId = characterId, ItemId = itemId, Quantity = quantity };
    }

    public void AddQuantity(int amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive.");
        Quantity += amount;
        SetUpdatedAt();
    }

    public void RemoveQuantity(int amount)
    {
        if (amount <= 0 || amount > Quantity)
            throw new InvalidOperationException("Cannot remove more than available quantity.");
        Quantity -= amount;
        SetUpdatedAt();
    }

    public void Equip()
    {
        IsEquipped = true;
        SetUpdatedAt();
    }

    public void Unequip()
    {
        IsEquipped = false;
        SetUpdatedAt();
    }
}
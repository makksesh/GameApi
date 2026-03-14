namespace Domain.ValueObjects;

public sealed class Money : IEquatable<Money>
{
    public decimal Amount { get; }

    private Money() { } 

    public Money(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));
        Amount = amount;
    }

    public Money Add(Money other) => new(Amount + other.Amount);
    public Money Subtract(Money other)
    {
        if (Amount < other.Amount)
            throw new InvalidOperationException("Insufficient funds.");
        return new(Amount - other.Amount);
    }

    public bool IsEnoughFor(Money price) => Amount >= price.Amount;

    public static Money Zero => new(0);

    public bool Equals(Money? other) => other is not null && Amount == other.Amount;
    public override bool Equals(object? obj) => obj is Money m && Equals(m);
    public override int GetHashCode() => Amount.GetHashCode();
    public override string ToString() => $"{Amount:F2} G";
}
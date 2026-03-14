namespace Domain.ValueObjects;

public sealed class StatBlock : IEquatable<StatBlock>
{
    public int Health { get; }
    public int Mana { get; }
    public int Armor { get; }
    public int Damage { get; }

    private StatBlock() { }

    public StatBlock(int health, int mana, int armor, int damage)
    {
        if (health <= 0) throw new ArgumentException("Health must be positive.", nameof(health));
        if (mana < 0)    throw new ArgumentException("Mana cannot be negative.",  nameof(mana));
        if (armor < 0)   throw new ArgumentException("Armor cannot be negative.", nameof(armor));
        if (damage <= 0) throw new ArgumentException("Damage must be positive.",  nameof(damage));

        Health = health;
        Mana   = mana;
        Armor  = armor;
        Damage = damage;
    }

    public static StatBlock Default => new(100, 50, 10, 15);
    
    public StatBlock WithUpgrade(int health = 0, int mana = 0, int armor = 0, int damage = 0)
        => new(Health + health, Mana + mana, Armor + armor, Damage + damage);

    public bool Equals(StatBlock? other)
        => other is not null
           && Health == other.Health && Mana == other.Mana
           && Armor  == other.Armor  && Damage == other.Damage;

    public override bool Equals(object? obj) => obj is StatBlock s && Equals(s);
    public override int GetHashCode() => HashCode.Combine(Health, Mana, Armor, Damage);
}
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Name).IsRequired().HasMaxLength(100);
        builder.Property(i => i.Description).HasMaxLength(500);
        builder.Property(i => i.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(i => i.Rarity).HasConversion<string>().HasMaxLength(20);

        builder.OwnsOne(i => i.BasePrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("BasePrice")
                .HasColumnType("numeric(18,2)");
        });
        
        builder.OwnsOne(i => i.BonusStats, stats =>
        {
            stats.Property(s => s.Health).HasColumnName("BonusHealth");
            stats.Property(s => s.Mana).HasColumnName("BonusMana");
            stats.Property(s => s.Armor).HasColumnName("BonusArmor");
            stats.Property(s => s.Damage).HasColumnName("BonusDamage");
        });
    }
}
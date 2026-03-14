using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CharacterConfiguration : IEntityTypeConfiguration<Character>
{
    public void Configure(EntityTypeBuilder<Character> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Level)
            .HasDefaultValue(1);
        
        builder.OwnsOne(c => c.Stats, stats =>
        {
            stats.Property(s => s.Health).HasColumnName("Health").IsRequired();
            stats.Property(s => s.Mana).HasColumnName("Mana").IsRequired();
            stats.Property(s => s.Armor).HasColumnName("Armor").IsRequired();
            stats.Property(s => s.Damage).HasColumnName("Damage").IsRequired();
        });
        
        builder.OwnsOne(c => c.Balance, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Balance")
                .HasColumnType("numeric(18,2)")
                .IsRequired();
        });

        builder.HasMany(c => c.Skills)
            .WithOne()
            .HasForeignKey(cs => cs.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Inventory)
            .WithOne()
            .HasForeignKey(ii => ii.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.HasKey(ii => ii.Id);

        builder.Property(ii => ii.Quantity).IsRequired();
        builder.Property(ii => ii.IsEquipped).HasDefaultValue(false);

        builder.HasOne(ii => ii.Item)
            .WithMany()
            .HasForeignKey(ii => ii.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ii => new { ii.CharacterId, ii.ItemId }).IsUnique();
    }
}
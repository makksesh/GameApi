using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class TradeLotConfiguration : IEntityTypeConfiguration<TradeLot>
{
    public void Configure(EntityTypeBuilder<TradeLot> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Quantity).IsRequired();
        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);

        builder.OwnsOne(t => t.Price, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Price")
                .HasColumnType("numeric(18,2)");
        });

        builder.HasOne(t => t.Item)
            .WithMany()
            .HasForeignKey(t => t.ItemId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(t => t.Status);
    }
}
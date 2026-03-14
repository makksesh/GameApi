using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.HasKey(p => p.Id);

        builder.OwnsOne(p => p.TotalPrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("TotalPrice")
                .HasColumnType("numeric(18,2)");
        });

        builder.HasOne<TradeLot>()
            .WithMany()
            .HasForeignKey(p => p.TradeLotId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
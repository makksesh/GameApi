using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class FriendRequestConfiguration : IEntityTypeConfiguration<FriendRequest>
{
    public void Configure(EntityTypeBuilder<FriendRequest> builder)
    {
        builder.HasKey(fr => fr.Id);

        builder.Property(fr => fr.Status)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        builder.HasIndex(fr => new { fr.SenderId, fr.ReceiverId }).IsUnique();
    }
}
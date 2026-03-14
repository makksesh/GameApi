using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class SupportTicketConfiguration : IEntityTypeConfiguration<SupportTicket>
{
    public void Configure(EntityTypeBuilder<SupportTicket> builder)
    {
        builder.HasKey(st => st.Id);

        builder.Property(st => st.Subject).IsRequired().HasMaxLength(200);
        builder.Property(st => st.Message).IsRequired().HasMaxLength(2000);
        builder.Property(st => st.Resolution).HasMaxLength(2000);

        builder.Property(st => st.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasIndex(st => st.Status);
        builder.HasIndex(st => st.AuthorId);
    }
}
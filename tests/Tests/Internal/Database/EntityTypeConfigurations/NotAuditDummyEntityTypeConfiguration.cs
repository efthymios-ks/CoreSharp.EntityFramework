using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tests.Internal.Database.Models;

namespace Domain.Database.EntityTypeConfigurations;

internal sealed class NotAuditDummyEntityTypeConfiguration : IEntityTypeConfiguration<NotAuditDummyEntity>
{
    // Constructors
    public void Configure(EntityTypeBuilder<NotAuditDummyEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .HasKey(dummy => dummy.Id);

        builder
            .Property(dummy => dummy.Id)
            .IsRequired();

        builder
            .Property(dummy => dummy.Name)
            .IsRequired()
            .HasMaxLength(100);
    }
}

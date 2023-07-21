using Domain.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Domain.Database.EntityTypeConfigurations;

internal sealed class StudentAddressEntityTypeConfiguration : IEntityTypeConfiguration<StudentAddress>
{
    // Constructors
    public void Configure(EntityTypeBuilder<StudentAddress> builder)
        => ArgumentNullException.ThrowIfNull(builder);
}

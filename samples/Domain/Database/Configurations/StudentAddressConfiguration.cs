using CoreSharp.EntityFramework.Samples.Domain.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace CoreSharp.EntityFramework.Samples.Domain.Database.Configurations;

internal class StudentAddressConfiguration : IEntityTypeConfiguration<StudentAddress>
{
    //Constructors
    public void Configure(EntityTypeBuilder<StudentAddress> builder)
        => _ = builder ?? throw new ArgumentNullException(nameof(builder));
}

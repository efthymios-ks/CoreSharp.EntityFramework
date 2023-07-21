using CoreSharp.EntityFramework.Extensions;
using Domain.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Domain.Database.EntityTypeConfigurations;

internal sealed class CourseEntityTypeConfiguration : IEntityTypeConfiguration<Course>
{
    // Constructors
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Property(course => course.Name)
               .IsRequired()
               .HasMaxLength(100);

        // Many-to-many enums
        builder.HasEnums(course => course.Fields);
    }
}

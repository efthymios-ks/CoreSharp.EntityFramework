using CoreSharp.EntityFramework.Extensions;
using Domain.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Domain.Database.EntityTypeConfigurations;

internal sealed class TeacherEntityTypeConfiguration : IEntityTypeConfiguration<Teacher>
{
    // Constructors
    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .Property(teacher => teacher.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Teachers - Courses > One-to-many
        builder
            .HasMany(teacher => teacher.Courses)
            .WithOne(course => course.Teacher)
            .HasForeignKey(course => course.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

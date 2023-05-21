﻿using Domain.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Domain.Database.Configurations;

internal sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    // Constructors
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Property(student => student.Name)
               .IsRequired()
               .HasMaxLength(100);

        // Student - Address > One-to-one
        builder.HasOne(student => student.Address)
               .WithOne(address => address.Student)
               .HasForeignKey<Student>(student => student.AddressId)
               .HasPrincipalKey<StudentAddress>(address => address.StudentId)
               .OnDelete(DeleteBehavior.Restrict);

        // Student - Courses > Many-to-many 
        builder.HasMany(s => s.Courses)
               .WithMany(c => c.Students)
               .UsingEntity(join => join.ToTable("StudentsCourses"));
    }
}

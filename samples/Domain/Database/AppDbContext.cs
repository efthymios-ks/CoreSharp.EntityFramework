using CoreSharp.EntityFramework.DbContexts.Common;
using CoreSharp.EntityFramework.Extensions;
using CoreSharp.EntityFramework.Samples.Domain.Database.Models;
using CoreSharp.EntityFramework.Samples.Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using System;

namespace CoreSharp.EntityFramework.Samples.Domain.Database;

public class AppDbContext : TrackableDbContextBase
{
    //Properties
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<StudentAddress> StudentAddresses { get; set; }
    public DbSet<Course> Courses { get; set; }

    //Methods
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.ConfigureSql();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Always call base method 
        base.OnModelCreating(modelBuilder);

        ConfigureEnums(modelBuilder);
        ConfigureModels(modelBuilder);
    }

    private static void ConfigureEnums(ModelBuilder modelBuilder)
    {
        modelBuilder.HasEnum<TeacherType>();
        modelBuilder.HasEnum<CourseField>();
    }

    private static void ConfigureModels(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}

using CoreSharp.EntityFramework.DbContexts.Abstracts;
using Domain.Database.Models;
using Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Domain.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options, ILoggerFactory? loggerFactory = null) : AuditDbContextBase(options)
{
    // Fields
    private readonly ILoggerFactory? _loggerFactory = loggerFactory;

    // Properties
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<StudentAddress> StudentAddresses { get; set; }
    public DbSet<Course> Courses { get; set; }

    // Methods
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.ConfigureSql(_loggerFactory!);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Always call base method 
        base.OnModelCreating(modelBuilder);

        ConfigureModels(modelBuilder);
    }

    private static void ConfigureModels(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}

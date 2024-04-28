using CoreSharp.EntityFramework.DbContexts.Abstracts;
using Microsoft.EntityFrameworkCore;
using Tests.Internal.Database.Models;

namespace Tests.Internal.Database.DbContexts;

public sealed class DummyDbContext : AuditDbContextBase
{
    public DummyDbContext(DbContextOptions<DummyDbContext> options)
        : base(options)
    {
    }

    // Properties
    public DbSet<DummyEntity> Dummies { get; set; }

    // Methods 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureModels(modelBuilder);
    }

    private static void ConfigureModels(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(DummyDbContext).Assembly);
}

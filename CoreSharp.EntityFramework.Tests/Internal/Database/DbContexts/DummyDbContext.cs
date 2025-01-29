using CoreSharp.EntityFramework.DbContexts.Abstracts;
using CoreSharp.EntityFramework.Tests.Internal.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Tests.Internal.Database.DbContexts;

public sealed class DummyDbContext(DbContextOptions<DummyDbContext> options) : AuditDbContextBase(options)
{

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

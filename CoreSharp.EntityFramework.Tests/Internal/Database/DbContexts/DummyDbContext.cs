using CoreSharp.EntityFramework.DbContexts.Abstracts;
using CoreSharp.EntityFramework.Tests.Internal.Database.DbContexts.Models;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Tests.Internal.Database.DbContexts;

public sealed class DummyDbContext(DbContextOptions<DummyDbContext> options) : AuditDbContextBase(options)
{
    // Properties
    public DbSet<DummyEntity> Dummies { get; set; }
    public DbSet<DummyEntityWithoutPrimaryKey> DummiesWithoutPrimaryKey { get; set; }
    public DbSet<DummyWithPrimaryKeyAndNoValueGeneration> DummiesWithPrimaryKeyAndNoValueGeneration { get; set; }
}

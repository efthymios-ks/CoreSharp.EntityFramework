using CoreSharp.EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Diagnostics.CodeAnalysis;
using Tests.Internal.Database.DbContexts;
using Tests.Internal.Database.Models;

namespace Tests.Internal.Abstracts;

public abstract class DummyDbContextTestsBase
{
    // Properties
    [SuppressMessage(
        "Structure", "NUnit1032:An IDisposable field/property should be Disposed in a TearDown method",
        Justification = "<Pending>")]
    internal static DummyDbContext DbContext
    {
        get => DummyMsSqlContainerSetup.DbContext;
        set => DummyMsSqlContainerSetup.DbContext = value;
    }

    // Methods 
    [SetUp]
    public async Task SetUpAsync()
    {
        if (DbContext is null || DbContext.IsDisposed)
        {
            var options = new DbContextOptionsBuilder<DummyDbContext>()
              .UseSqlServer(DummyMsSqlContainerSetup.SqlConnectionString)
              .EnableDetailedErrors()
              .EnableSensitiveDataLogging()
              .Options;
            DbContext = new DummyDbContext(options);
            await DbContext.Database.EnsureCreatedAsync();
        }

        await DbContext.RollbackAsync();
        DbContext.ChangeTracker.DetectChanges();
        DbContext.ChangeTracker.Clear();
        await DbContext.Database.ExecuteSqlAsync($"DELETE FROM Dummies");
    }

    protected static DummyEntity GenerateDummy()
        => new()
        {
            Name = Guid.NewGuid().ToString()
        };

    protected static DummyEntity[] GenerateDummies(int count)
        => Enumerable
            .Range(0, count)
            .Select(_ => GenerateDummy())
            .ToArray();

    protected static async Task<DummyEntity> PreloadDummyAsync()
        => (await PreloadDummiesAsync(1))[0];

    protected static async Task<DummyEntity[]> PreloadDummiesAsync(int count)
    {
        var dummies = GenerateDummies(count);
        await DbContext.Dummies.AddRangeAsync(dummies);
        await DbContext.SaveChangesAsync();
        return dummies;
    }

    protected static EntityEntry<TEntity> GetEntry<TEntity>(EntityState entityState)
        where TEntity : class
        => GetEntries<TEntity>(entityState)
            .FirstOrDefault();

    protected static EntityEntry<DummyEntity> GetDummyEntry(EntityState entityState)
        => GetDummyEntries(entityState)
            .FirstOrDefault();

    protected static EntityEntry<TEntity>[] GetEntries<TEntity>(EntityState entityState)
        where TEntity : class
        => DbContext
            .ChangeTracker
            .Entries<TEntity>()
            .Where(entry => entry.State == entityState)
            .ToArray();

    protected static EntityEntry<DummyEntity>[] GetDummyEntries(EntityState entityState)
        => GetEntries<DummyEntity>(entityState);
}

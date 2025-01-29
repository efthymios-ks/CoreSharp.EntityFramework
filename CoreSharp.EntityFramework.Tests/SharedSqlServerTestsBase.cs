using CoreSharp.EntityFramework.Extensions;
using CoreSharp.EntityFramework.Tests.Internal.Database.DbContexts;
using CoreSharp.EntityFramework.Tests.Internal.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CoreSharp.EntityFramework.Tests;

public abstract class SharedSqlServerTestsBase(SharedSqlServerContainer sqlContainer) : IAsyncLifetime
{
    private readonly SharedSqlServerContainer _sqlContainer = sqlContainer;

    protected DummyDbContext DbContext
    {
        get => _sqlContainer.DbContext;
        set => _sqlContainer.DbContext = value;
    }

    public async Task InitializeAsync()
    {
        /*
            Try initialize on each test, ONLY IF NEEDED,
            because DbContext is shared across all tests and some tests dispose it.
        */
        if (DbContext is null || DbContext.IsDisposed)
        {
            var options = new DbContextOptionsBuilder<DummyDbContext>()
                .UseSqlServer(_sqlContainer.SqlConnectionString)
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .Options;

            DbContext = new DummyDbContext(options);
            await DbContext.Database.EnsureCreatedAsync();
        }

        await DbContext.RollbackAsync();
        DbContext.ChangeTracker.DetectChanges();
        DbContext.ChangeTracker.Clear();
        await DbContext.Database.ExecuteSqlAsync($"TRUNCATE TABLE Dummies");
    }

    public Task DisposeAsync()
        => Task.CompletedTask;

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

    protected async Task<DummyEntity> PreloadDummyAsync()
        => (await PreloadDummiesAsync(1))[0];

    protected async Task<DummyEntity[]> PreloadDummiesAsync(int count)
    {
        var dummies = GenerateDummies(count);
        await DbContext.Dummies.AddRangeAsync(dummies);
        await DbContext.SaveChangesAsync();
        return dummies;
    }

    protected EntityEntry<TEntity>? GetEntry<TEntity>(EntityState entityState)
        where TEntity : class
        => GetEntries<TEntity>(entityState)
            .FirstOrDefault();

    protected EntityEntry<DummyEntity>? GetDummyEntry(EntityState entityState)
        => GetDummyEntries(entityState)
            .FirstOrDefault();

    protected EntityEntry<TEntity>[] GetEntries<TEntity>(EntityState entityState)
        where TEntity : class
        => DbContext
            .ChangeTracker
            .Entries<TEntity>()
            .Where(entry => entry.State == entityState)
            .ToArray();

    protected EntityEntry<DummyEntity>[] GetDummyEntries(EntityState entityState)
        => GetEntries<DummyEntity>(entityState);
}

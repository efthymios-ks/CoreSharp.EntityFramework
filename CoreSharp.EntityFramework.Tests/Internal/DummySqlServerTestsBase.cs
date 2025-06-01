using CoreSharp.EntityFramework.Tests.Internal.Database.DbContexts;
using CoreSharp.EntityFramework.Tests.Internal.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CoreSharp.EntityFramework.Tests.Internal;

public abstract class DummySqlServerTestsBase(DummySqlServerContainer sqlContainer) : IAsyncLifetime
{
    private readonly DummySqlServerContainer _sqlContainer = sqlContainer;

    protected DummyDbContext DummyDbContext { get; set; } = null!;

    protected string SqlConnectionString
        => _sqlContainer.SqlConnectionString;

    public async Task InitializeAsync()
    {
        /*
            Try initialize on each test, ONLY IF NEEDED,
            because DbContext is shared across all tests and some tests dispose it.
        */
        if (DummyDbContext is null || DummyDbContext.IsDisposed)
        {
            var options = new DbContextOptionsBuilder<DummyDbContext>()
                .UseSqlServer(_sqlContainer.SqlConnectionString)
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .Options;

            DummyDbContext = new DummyDbContext(options);
            await DummyDbContext.Database.EnsureCreatedAsync();
            await DummyDbContext.EnsureValueGeneratedConstraintsAsync();
        }

        await DummyDbContext.Database.ExecuteSqlAsync($"TRUNCATE TABLE Dummies");
        DummyDbContext.ChangeTracker.DetectChanges();
        DummyDbContext.ChangeTracker.Clear();
    }

    public Task DisposeAsync()
        => Task.CompletedTask;

    protected static DummyEntity GenerateDummy()
        => new()
        {
            Name = Guid.NewGuid().ToString()
        };

    protected static DummyEntity[] GenerateDummies(int count)
        => [.. Enumerable
            .Range(0, count)
            .Select(_ => GenerateDummy())
        ];

    protected async Task<DummyEntity> PreloadDummyAsync()
        => (await PreloadDummiesAsync(1))[0];

    protected async Task<DummyEntity[]> PreloadDummiesAsync(int count)
    {
        var dummies = GenerateDummies(count);
        await DummyDbContext.Dummies.AddRangeAsync(dummies);
        await DummyDbContext.SaveChangesAsync();
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
        => [.. DummyDbContext
            .ChangeTracker
            .Entries<TEntity>()
            .Where(entry => entry.State == entityState)
        ];

    protected EntityEntry<DummyEntity>[] GetDummyEntries(EntityState entityState)
        => GetEntries<DummyEntity>(entityState);
}

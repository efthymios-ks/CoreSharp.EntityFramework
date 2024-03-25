using Microsoft.EntityFrameworkCore;
using Tests.Internal.Database.DbContexts;
using Tests.Internal.Database.Models;

namespace Tests.Internal.Abstracts;

public abstract class DummyDbContextTestsBase
{
    // Properties
    internal static DummyDbContext DbContext { get; set; }

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

        await DbContext.Database.ExecuteSqlAsync($"DELETE FROM Dummies");
    }

    protected static DummyEntity[] GenerateDummies(int count)
        => Enumerable
            .Range(0, count)
            .Select(_ => GenerateDummy())
            .ToArray();

    protected static DummyEntity GenerateDummy()
        => new()
        {
            Name = Guid.NewGuid().ToString()
        };

    protected static async Task<DummyEntity[]> PreloadDummiesAsync(int count)
    {
        var dummies = GenerateDummies(count);
        await DbContext.AddRangeAsync(dummies);
        await DbContext.SaveChangesAsync();
        return dummies;
    }
}

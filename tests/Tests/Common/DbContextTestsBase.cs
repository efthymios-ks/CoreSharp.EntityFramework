using Domain.Database;
using Microsoft.EntityFrameworkCore;

namespace Tests.Common;

public abstract class DbContextTestsBase
{
    // Methods
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var databaseName = $"{nameof(AppDbContext)}_{DateTime.Now.ToFileTimeUtc()}";
        var options = new DbContextOptionsBuilder<AppDbContext>()
                        .UseInMemoryDatabase(databaseName: databaseName)
                        .Options;
        DbContext = new AppDbContext(options);
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDownAsync()
        => await DbContext.DisposeAsync();

    [SetUp]
    public async Task SetUpAsync()
        => await DbContext.Database.EnsureCreatedAsync();

    [TearDown]
    public async Task TearDownAsync()
        => await DbContext.Database.EnsureDeletedAsync();

    // Properties
    protected AppDbContext DbContext { get; set; }
}

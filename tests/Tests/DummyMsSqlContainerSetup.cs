using Testcontainers.MsSql;
using Tests.Internal.Database.DbContexts;

namespace Tests;

[SetUpFixture]
public sealed class DummyMsSqlContainerSetup
{
    private static readonly MsSqlContainer _sqlContainer = new MsSqlBuilder()
        .Build();

    private static DummyDbContext DbContext
        => DummyDbContextTestsBase.DbContext;

    internal static string SqlConnectionString
        => _sqlContainer.GetConnectionString();

    [OneTimeSetUp]
    public Task OneTimeSetUpAsync()
        => _sqlContainer.StartAsync();

    [OneTimeTearDown]
    public async Task OneTimeTearDownAsync()
    {
        if (DbContext is not null)
        {
            await DbContext.DisposeAsync();
        }

        await _sqlContainer.StopAsync();
    }
}

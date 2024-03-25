using Testcontainers.MsSql;
using Tests.Internal.Database.DbContexts;

namespace Tests;

[SetUpFixture]
public static class DummyMsSqlContainerSetup
{
    private static readonly MsSqlContainer _sqlContainer = new MsSqlBuilder().Build();

    internal static DummyDbContext DbContext { get; set; }

    internal static string SqlConnectionString
        => _sqlContainer.GetConnectionString();

    [OneTimeSetUp]
    public static Task OneTimeSetUpAsync()
        => _sqlContainer.StartAsync();

    [OneTimeTearDown]
    public async static Task OneTimeTearDownAsync()
    {
        if (DbContext is not null)
        {
            await DbContext.DisposeAsync();
        }

        await _sqlContainer.StopAsync();
        await _sqlContainer.DisposeAsync();
    }
}

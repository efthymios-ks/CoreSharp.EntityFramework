using Domain.Database;
using Testcontainers.MsSql;

namespace Tests;

[SetUpFixture]
public sealed class MsSqlContainerSetup
{
    private static readonly MsSqlContainer _sqlContainer = new MsSqlBuilder()
        .Build();

    private static AppDbContext AppDbContext
        => AppDbContextTestsBase.AppDbContext;

    internal static string SqlConnectionString
        => _sqlContainer.GetConnectionString();

    [OneTimeSetUp]
    public Task OneTimeSetUpAsync()
        => _sqlContainer.StartAsync();

    [OneTimeTearDown]
    public async Task OneTimeTearDownAsync()
    {
        if (AppDbContext is not null)
        {
            await AppDbContext.DisposeAsync();
        }

        await _sqlContainer.StopAsync();
    }
}

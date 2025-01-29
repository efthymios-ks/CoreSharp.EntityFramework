using CoreSharp.EntityFramework.Tests.Internal.Database.DbContexts;
using Testcontainers.MsSql;

namespace CoreSharp.EntityFramework.Tests;

public sealed class SharedSqlServerContainer : IAsyncLifetime
{
    private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder().Build();

    public DummyDbContext DbContext { get; set; } = null!;

    internal string SqlConnectionString
        => _sqlContainer.GetConnectionString();

    public async Task InitializeAsync()
        => await _sqlContainer.StartAsync();

    public async Task DisposeAsync()
    {
        if (DbContext is not null)
        {
            await DbContext.DisposeAsync();
        }

        await _sqlContainer.StopAsync();
        await _sqlContainer.DisposeAsync();
    }
}

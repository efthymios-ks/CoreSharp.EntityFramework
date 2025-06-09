using Testcontainers.MsSql;

namespace CoreSharp.EntityFramework.Tests.Internal.Database;

public sealed class DummySqlServerContainer : IAsyncLifetime
{
    private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder().Build();

    internal string SqlConnectionString
        => _sqlContainer.GetConnectionString();

    public async Task InitializeAsync()
        => await _sqlContainer.StartAsync();

    public async Task DisposeAsync()
    {
        await _sqlContainer.StopAsync();
        await _sqlContainer.DisposeAsync();
    }
}

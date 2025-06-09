using CoreSharp.EntityFramework.Bulkoperations;
using CoreSharp.EntityFramework.Tests.Internal.Database;
using CoreSharp.EntityFramework.Tests.Internal.Database.DbContexts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CoreSharp.EntityFramework.Tests.BulkOperations;

[Collection(nameof(DummySqlServerCollection))]
public sealed class SqlConnectionWrapperTests(DummySqlServerContainer sqlContainer)
    : DummySqlServerTestsBase(sqlContainer)
{
    [Fact]
    public async Task EnsureConnectAsync_WhenDbContextIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DummyDbContext? dbContext = null;

        // Act
        Task Action()
            => SqlConnectionWrapper.EnsureConnectAsync(dbContext!);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task EnsureConnectAsync_WhenDbContextProviderIsNotSqlServer_ShouldThrowInvalidOperationException()
    {
        // Arrange
        using var dbContext = new DummyDbContext(new DbContextOptionsBuilder<DummyDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options
        );

        // Act
        Task Action()
            => SqlConnectionWrapper.EnsureConnectAsync(dbContext);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Action);
    }

    [Fact]
    public async Task EnsureConnectAsync_WhenConnectionIsClosed_ShouldOpenConnectionAndOwnIt()
    {
        // Arrange
        await ForceCloseConnectionAsync(DummyDbContext);

        // Act
        await using var wrapper = await SqlConnectionWrapper.EnsureConnectAsync(DummyDbContext);

        // Assert
        Assert.NotNull(wrapper.Connection);
        Assert.Equal(ConnectionState.Open, wrapper.Connection.State);
        Assert.True(wrapper.IsConnectionOwned);
        Assert.NotNull(wrapper.Transaction);
        Assert.True(wrapper.IsTransactionOwned);
    }

    [Fact]
    public async Task EnsureConnectAsync_WhenConnectionIsOpen_ShouldExistingConnectionAndNotOwnIt()
    {
        // Arrange
        await DummyDbContext.Database.OpenConnectionAsync();

        // Act
        await using var wrapper = await SqlConnectionWrapper.EnsureConnectAsync(DummyDbContext);

        // Assert
        Assert.NotNull(wrapper.Connection);
        Assert.Equal(ConnectionState.Open, wrapper.Connection.State);
        Assert.False(wrapper.IsConnectionOwned);
        Assert.NotNull(wrapper.Transaction);
        Assert.True(wrapper.IsTransactionOwned);
    }

    [Fact]
    public async Task EnsureConnectAsync_WhenTransactionIsNull_ShouldReturnNewTransactionAndOwnIt()
    {
        // Act 
        await using var wrapper = await SqlConnectionWrapper.EnsureConnectAsync(DummyDbContext);

        // Assert
        Assert.NotNull(wrapper.Connection);
        Assert.Equal(ConnectionState.Open, wrapper.Connection.State);
        Assert.True(wrapper.IsConnectionOwned);
        Assert.NotNull(wrapper.Transaction);
        Assert.True(wrapper.IsTransactionOwned);
    }

    [Fact]
    public async Task EnsureConnectAsync_WhenTransactionIsNotNull_ShouldReturnExistingTransactionAndNotOwnIt()
    {
        // Arrange
        using var efTransaction = await DummyDbContext.Database.BeginTransactionAsync();

        // Act 
        await using var wrapper = await SqlConnectionWrapper.EnsureConnectAsync(DummyDbContext);

        // Assert
        Assert.NotNull(wrapper.Connection);
        Assert.Equal(ConnectionState.Open, wrapper.Connection.State);
        Assert.False(wrapper.IsConnectionOwned);
        Assert.NotNull(wrapper.Transaction);
        Assert.False(wrapper.IsTransactionOwned);
    }

    [Fact]
    public async Task DisposeAsync_WhenTransactionIsOwned_ShouldDisposeTransaction()
    {
        // Act
        var isConnectionOwned = false;
        var isTransactionOwned = false;
        SqlTransaction transaction = null!;
        await using (var wrapper = await SqlConnectionWrapper.EnsureConnectAsync(DummyDbContext))
        {
            isConnectionOwned = wrapper.IsConnectionOwned;
            isTransactionOwned = wrapper.IsTransactionOwned;
            transaction = wrapper.Transaction;
        }

        // Assert 
        Assert.True(isTransactionOwned);
        await Assert.ThrowsAsync<InvalidOperationException>(() => transaction!.CommitAsync());
    }

    [Fact]
    public async Task DisposeAsync_WhenTransactionIsNotOwned_ShouldNotDisposeTransaction()
    {
        // Arrange
        using var efTransaction = await DummyDbContext.Database.BeginTransactionAsync();

        // Act
        var isConnectionOwned = false;
        var isTransactionOwned = false;
        SqlTransaction transaction = null!;
        await using (var wrapper = await SqlConnectionWrapper.EnsureConnectAsync(DummyDbContext))
        {
            isConnectionOwned = wrapper.IsConnectionOwned;
            isTransactionOwned = wrapper.IsTransactionOwned;
            transaction = wrapper.Transaction;
        }

        var transactionException = await Record.ExceptionAsync(() => transaction!.CommitAsync());

        // Assert 
        Assert.False(isTransactionOwned);
        Assert.NotNull(transaction);
        Assert.Null(transactionException);
    }

    [Fact]
    public async Task DisposeAsync_WhenConnectionIsOwned_ShouldCloseConnection()
    {
        // Arrange
        await ForceCloseConnectionAsync(DummyDbContext);

        // Arrange
        await using var wrapper = await SqlConnectionWrapper.EnsureConnectAsync(DummyDbContext);

        // Act
        await wrapper.DisposeAsync();

        // Assert
        Assert.True(wrapper.IsConnectionOwned);
        Assert.Equal(ConnectionState.Closed, wrapper.Connection.State);
    }

    [Fact]
    public async Task DisposeAsync_WhenConnectionIsNotOwned_ShouldNotCloseConnection()
    {
        // Arrange
        await DummyDbContext.Database.OpenConnectionAsync();

        // Act
        var isConnectionOwned = false;
        SqlConnection sqlConnection = null!;
        await using (var wrapper = await SqlConnectionWrapper.EnsureConnectAsync(DummyDbContext))
        {
            isConnectionOwned = wrapper.IsConnectionOwned;
            sqlConnection = wrapper.Connection;
        }

        // Assert
        Assert.False(isConnectionOwned);
        Assert.Equal(ConnectionState.Open, sqlConnection.State);
    }

    [Fact]
    public async Task DisposeAsync_WhenCalledTwice_ShouldNotThrowException()
    {
        // Arrange
        await using var wrapper = await SqlConnectionWrapper.EnsureConnectAsync(DummyDbContext);

        // Act
        await wrapper.DisposeAsync();
        var exception = await Record.ExceptionAsync(() => wrapper.DisposeAsync().AsTask());

        // Assert
        Assert.Null(exception);
    }

    private static async Task ForceCloseConnectionAsync(DbContext dbContext)
    {
        var retries = 0;
        var connection = dbContext.Database.GetDbConnection();
        while (!connection.State.HasFlag(ConnectionState.Closed))
        {
            await dbContext.Database.CloseConnectionAsync();
            retries++;

            if (retries > 100)
            {
                throw new InvalidOperationException("Failed to close the connection after multiple attempts.");
            }
        }
    }
}

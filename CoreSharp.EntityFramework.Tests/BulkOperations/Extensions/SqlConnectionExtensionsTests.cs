using CoreSharp.EntityFramework.Bulkoperations.Extensions;
using CoreSharp.EntityFramework.Tests.Internal.Database;
using CoreSharp.EntityFramework.Tests.Internal.Database.DbContexts.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CoreSharp.EntityFramework.Tests.BulkOperations.Extensions;

[Collection(nameof(DummySqlServerCollection))]
public sealed class SqlConnectionExtensionsTests(DummySqlServerContainer sqlContainer)
    : DummySqlServerTestsBase(sqlContainer)
{
    [Fact]
    public async Task BulkCopyAsync_WhenConnectionIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        SqlConnection? connection = null;
        using var dummyTable = GetDummyTable();

        // Act
        Task Action()
            => connection!.BulkCopyAsync(dummyTable.TableName, dummyTable);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task BulkCopyAsync_WhenTableNameIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        using var connection = new SqlConnection(SqlConnectionString);
        using var dummyTable = GetDummyTable();

        // Act
        Task Action()
            => connection.BulkCopyAsync(tableName: null!, dummyTable);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task BulkCopyAsync_WhenTableNameIsEmptyOrWhitespace_ShouldThrowArgumentException(string tableName)
    {
        // Arrange
        using var connection = new SqlConnection(SqlConnectionString);
        using var dummyTable = GetDummyTable();

        // Act
        Task Action()
            => connection.BulkCopyAsync(tableName, dummyTable);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(Action);
    }

    [Fact]
    public async Task BulkCopyAsync_WhenDataTableIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        using var connection = new SqlConnection(SqlConnectionString);

        // Act
        Task Action()
            => connection.BulkCopyAsync("Dummies", dataTable: null!);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task BulkCopyAsync_WhenConnectionIsClosed_ShouldOpenConnectionAndCloseItAfterwards()
    {
        // Arrange
        using var connection = new SqlConnection(SqlConnectionString);
        using var dummyTable = GetDummyTable();

        var connectionChanges = new List<ConnectionState>();
        connection.StateChange += (sender, args)
            => connectionChanges.Add(args.CurrentState);

        // Act
        await connection.BulkCopyAsync(dummyTable.TableName, dummyTable);

        // Assert
        Assert.Equal(ConnectionState.Closed, connection.State);
        Assert.Equivalent(new[] { ConnectionState.Open, ConnectionState.Closed }, connectionChanges);
    }

    [Fact]
    public async Task BulkCopyAsync_WhenConnectionIsOpen_ShouldNotChangeConnectionState()
    {
        // Arrange
        using var connection = new SqlConnection(SqlConnectionString);
        await connection.OpenAsync();
        using var dummyTable = GetDummyTable();

        var connectionChanges = new List<ConnectionState>();
        connection.StateChange += (sender, args)
            => connectionChanges.Add(args.CurrentState);

        // Act
        await connection.BulkCopyAsync(dummyTable.TableName, dummyTable);

        // Assert
        Assert.Equal(ConnectionState.Open, connection.State);
        Assert.Empty(connectionChanges);
    }

    [Fact]
    public async Task BulkCopyAsync_WhenCalled_ShouldInsertDataIntoTable()
    {
        // Arrange
        using var connection = new SqlConnection(SqlConnectionString);
        await connection.OpenAsync();
        using var dummyTable = GetDummyTable();
        dummyTable.Rows.Add(Guid.NewGuid(), "Name", DateTime.UtcNow);

        // Act
        await connection.BulkCopyAsync(dummyTable.TableName, dummyTable);

        // Assert
        using var checkCommand = new SqlCommand($"SELECT COUNT(*) FROM {dummyTable.TableName}", connection);
        var count = await checkCommand.ExecuteScalarAsync();
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task BulkCopyAsync_WhenTransactionIsProvided_ShouldUseTransaction()
    {
        // Arrange
        using var connection = new SqlConnection(SqlConnectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();
        using var dummyTable = GetDummyTable();
        dummyTable.Rows.Add(Guid.NewGuid(), "Name", DateTime.UtcNow);

        // Act
        await connection.BulkCopyAsync(dummyTable.TableName, dummyTable, transaction);
        transaction.Rollback();

        // Assert
        using var checkCommand = new SqlCommand($"SELECT COUNT(*) FROM {dummyTable.TableName}", connection);
        var count = await checkCommand.ExecuteScalarAsync();
        Assert.Equal(0, count);
    }

    private static DataTable GetDummyTable()
    {
        var dataTable = new DataTable("Dummies");
        var requiredProperties = new[]
        {
            nameof(DummyEntity.Id),
            nameof(DummyEntity.Name),
            nameof(DummyEntity.DateCreatedUtc)
        }.Select(propertyName => typeof(DummyEntity).GetProperty(propertyName))
        .ToArray();

        foreach (var propeprty in requiredProperties)
        {
            dataTable.Columns.Add(propeprty!.Name, propeprty.PropertyType);
        }

        return dataTable;
    }
}

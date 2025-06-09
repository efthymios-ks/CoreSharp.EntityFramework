using CoreSharp.EntityFramework.Bulkoperations.Extensions;
using CoreSharp.EntityFramework.Tests.Internal;
using CoreSharp.EntityFramework.Tests.Internal.Database;
using CoreSharp.EntityFramework.Tests.Internal.Database.DbContexts.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CoreSharp.EntityFramework.Tests.BulkOperations.Extensions;

[Collection(nameof(DummySqlServerCollection))]
public sealed class SqlTransactionExtensionsTests(DummySqlServerContainer sqlContainer)
    : DummySqlServerTestsBase(sqlContainer)
{
    [Fact]
    public async Task CreateAsync_WhenTransactionIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        SqlTransaction transaction = null!;
        using var dummyTable = GetDummyTable();

        // Act
        Task Action()
            => transaction.CreateTempTableAsync(dummyTable);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task CreateAsync_WhenDataTableIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        using var connection = new SqlConnection(SqlConnectionString);
        await connection.OpenAsync();
        using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();

        // Act
        Task Action()
            => transaction.CreateTempTableAsync(sourceTable: null!);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task CreateAsync_WhenCalled_ShouldReturnTableName()
    {
        // Arrange
        using var connection = new SqlConnection(SqlConnectionString);
        await connection.OpenAsync();
        using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();
        using var dummyTable = GetDummyTable();

        // Act
        var tempTableName = await transaction.CreateTempTableAsync(dummyTable);

        // Assert  
        Assert.NotEmpty(tempTableName);
        Assert.StartsWith("#Temp_", tempTableName);
    }

    [Fact]
    public async Task CreateAsync_WhenCalled_ShouldCreateAndPopulateTempTable()
    {
        // Arrange
        using var connection = new SqlConnection(SqlConnectionString);
        await connection.OpenAsync();
        using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();
        using var dummyTableToInsert = GetDummyTable();
        dummyTableToInsert.Rows.Add(Guid.NewGuid(), "Name", DateTime.UtcNow);

        // Act
        var tempTableName = await transaction.CreateTempTableAsync(dummyTableToInsert);

        // Assert
        var selectSql = $"SELECT * FROM {tempTableName}";
        using var selectCommand = new SqlCommand(selectSql, connection, transaction);
        using var reader = await selectCommand.ExecuteReaderAsync();
        using var tempTableRead = new DataTable();
        tempTableRead.Load(reader);
        AssertUtils.Equivalent(dummyTableToInsert, tempTableRead);
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

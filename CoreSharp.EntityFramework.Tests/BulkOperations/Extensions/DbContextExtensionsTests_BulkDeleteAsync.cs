using CoreSharp.EntityFramework.Bulkoperations.Extensions;
using CoreSharp.EntityFramework.Tests.Internal.Database;
using CoreSharp.EntityFramework.Tests.Internal.Database.DbContexts;
using CoreSharp.EntityFramework.Tests.Internal.Database.DbContexts.Models;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Tests.BulkOperations.Extensions;

[Collection(nameof(DummySqlServerCollection))]
public sealed class DbContextExtensionsTests_BulkDeleteAsync(DummySqlServerContainer sqlContainer)
    : DummySqlServerTestsBase(sqlContainer)
{
    [Fact]
    public async Task BulkDeleteAsync_WhenDbContextIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbContext? dbContext = null;
        var valuesToMatch = new object?[][]
        {
            [Guid.NewGuid()]
        };

        // Act
        Task Action()
            => dbContext!.BulkDeleteAsync<DummyEntity>(valuesToMatch);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task BulkDeleteAsync_WhenValuesToMatchIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        Task Action()
            => DummyDbContext.BulkDeleteAsync<DummyEntity>(valuesToMatch: null!);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task BulkDeleteAsync_WhenValuesToMatchIsEmpty_ShouldNotThrowException()
    {
        // Act
        var exception = await Record.ExceptionAsync(()
            => DummyDbContext.BulkDeleteAsync<DummyEntity>(valuesToMatch: []));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task BulkDeleteAsync_WhenOptionsIsNull_ShouldNotThrowException()
    {
        // Arrange
        var valuesToMatch = new object?[][]
        {
            [Guid.NewGuid()]
        };

        // Act
        var exception = await Record.ExceptionAsync(()
            => DummyDbContext.BulkDeleteAsync<DummyEntity>(valuesToMatch));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task BulkDeleteAsync_WhenEntityTypeIsNotPartOfDbContext_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var valuesToMatch = new object?[][]
        {
            [Guid.NewGuid()]
        };

        // Act
        Task Action()
            => DummyDbContext.BulkDeleteAsync<DbContextExtensionsTests_BulkInsertAsync>(valuesToMatch);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Action);
    }

    [Fact]
    public async Task BulkDeleteAsync_WhenEntityHasNoPrimaryKey_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var valuesToMatch = new object?[][]
        {
            [Guid.NewGuid()]
        };

        // Act
        Task Action()
            => DummyDbContext.BulkDeleteAsync<DummyEntityWithoutPrimaryKey>(valuesToMatch);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Action);
    }

    [Fact]
    public async Task BulkDeleteAsync_WhenValuesToMatchLenghtAndPropertiesToMatchLengthDiffer_ShouldThrowInvalidOperationExceptionn()
    {
        // Arrange
        var valuesToMatch = new object?[][]
        {
            [Guid.NewGuid(), Guid.NewGuid()]
        };

        // Act
        Task Action()
            => DummyDbContext.BulkDeleteAsync<DummyEntity>(valuesToMatch, options: new()
            {
                PropertiesToMatch = [dummy => dummy.Id]
            });

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Action);
    }

    [Fact]
    public async Task BulkDeleteAsync_WhenPropertiesToMatchIsEmpty_ShouldDeleteByPrimaryKey()
    {
        // Arrange
        var existingDummies = await PreloadDummiesAsync(2);
        var dummyToDelete = existingDummies[0];
        var dummyToKeep = existingDummies[1];
        var valuesToMatch = new object?[][]
        {
            [dummyToDelete.Id]
        };

        // Act
        await DummyDbContext.BulkDeleteAsync<DummyEntity>(valuesToMatch, options: new()
        {
            PropertiesToMatch = []
        });

        var dummiesAfterDelete = await DummyDbContext.Dummies.ToArrayAsync();

        // Assert
        var dummyAfterDelete = Assert.Single(dummiesAfterDelete);
        Assert.Equal(dummyToKeep.Id, dummyAfterDelete.Id);
        Assert.Equal(dummyToKeep.Name, dummyAfterDelete.Name);
    }

    [Fact]
    public async Task BulkDeleteAsync_WhenPropertiesToMatchIsNotEmpty_ShouldDeleteByPropertiesToMatch()
    {
        // Arrange
        var existingDummies = await PreloadDummiesAsync(2);
        var dummyToDelete = existingDummies[0];
        var dummyToKeep = existingDummies[1];
        var valuesToMatch = new object?[][]
        {
            [dummyToDelete.Name, null]
        };

        // Act
        await DummyDbContext.BulkDeleteAsync<DummyEntity>(valuesToMatch, options: new()
        {
            PropertiesToMatch = [
                dummy => dummy.Name,
                dummy => dummy.DateModifiedUtc
            ]
        });

        var dummiesAfterDelete = await DummyDbContext.Dummies.ToArrayAsync();

        // Assert
        var dummyAfterDelete = Assert.Single(dummiesAfterDelete);
        Assert.Equal(dummyToKeep.Id, dummyAfterDelete.Id);
        Assert.Equal(dummyToKeep.Name, dummyAfterDelete.Name);
    }
}

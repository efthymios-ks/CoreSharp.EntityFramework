using CoreSharp.EntityFramework.Bulkoperations.Extensions;
using CoreSharp.EntityFramework.Tests.Internal.Database;
using CoreSharp.EntityFramework.Tests.Internal.Database.DbContexts;
using CoreSharp.EntityFramework.Tests.Internal.Database.DbContexts.Models;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Tests.BulkOperations.Extensions;

[Collection(nameof(DummySqlServerCollection))]
public sealed class DbContextExtensionsTests_BulkUpdateAsync(DummySqlServerContainer sqlContainer)
    : DummySqlServerTestsBase(sqlContainer)
{
    [Fact]
    public async Task BulkUpdateAsync_WhenDbContextIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbContext? dbContext = null;
        var dummies = Array.Empty<DummyEntity>();

        // Act
        Task Action()
            => dbContext!.BulkUpdateAsync(dummies);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task BulkUpdateAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        Task Action()
            => DummyDbContext.BulkUpdateAsync<DummyEntity>(entities: null!);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task BulkUpdateAsync_WhenEntitiesIsEmpty_ShouldNotThrowException()
    {
        // Act
        var exception = await Record.ExceptionAsync(()
            => DummyDbContext.BulkUpdateAsync<DummyEntity>([]));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task BulkUpdateAsync_WhenOptionsIsNull_ShouldNotThrowException()
    {
        // Arrange
        var existingDummies = await PreloadDummiesAsync(1);

        // Act
        var exception = await Record.ExceptionAsync(()
            => DummyDbContext.BulkUpdateAsync(existingDummies, options: null!));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task BulkUpdateAsync_WhenEntityTypeIsNotPartOfDbContext_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var entities = new DbContextExtensionsTests_BulkInsertAsync[]
        {
            new(null!)
        };

        // Act
        Task Action()
            => DummyDbContext.BulkUpdateAsync(entities);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Action);
    }

    [Fact]
    public async Task BulkUpdateAsync_WhenEntityHasNoPrimaryKey_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var dummyWithoutPk = new DummyEntityWithoutPrimaryKey
        {
            Name = Guid.NewGuid().ToString()
        };

        // Act
        Task Action()
            => DummyDbContext.BulkUpdateAsync([dummyWithoutPk]);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Action);
    }

    [Fact]
    public async Task BulkUpdateAsync_WhenComputedPropertiesToUpdateIsEmpty_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var existingDummies = await PreloadDummiesAsync(1);

        // Act
        Task Action()
            => DummyDbContext.BulkUpdateAsync(existingDummies, options: new()
            {
                // Id is PK, so it will be filtered out and result to exception
                PropertiesToUpdate = [dummy => dummy.Id]
            });

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Action);
    }

    [Fact]
    public async Task BulkUpdateAsync_WhenPropertiesToMatchIsEmpty_ShouldMatchByPrimaryKey()
    {
        // Arrange
        var existingDummy = await PreloadDummyAsync();
        var dummyToUpdate = new DummyEntity
        {
            Id = existingDummy.Id,
            Name = Guid.NewGuid().ToString()
        };

        // Act
        await DummyDbContext.BulkUpdateAsync([dummyToUpdate], options: new()
        {
            PropertiesToMatch = []
        });
        var dummiesAfterUpdate = await DummyDbContext.Dummies.ToArrayAsync();

        // Assert
        var dummyAfterUpdate = Assert.Single(dummiesAfterUpdate);
        Assert.Equal(dummyToUpdate.Id, dummyAfterUpdate.Id);
        Assert.Equal(dummyToUpdate.Name, dummyAfterUpdate.Name);
    }

    [Fact]
    public async Task BulkUpdateAsync_WhenPropertiesToMatchIsNotEmpty_ShouldMatchBySpecifiedProperties()
    {
        // Arrange
        var existingDummy = await PreloadDummyAsync();
        var dummyToUpdate = new DummyEntity
        {
            Id = existingDummy.Id,
            Name = Guid.NewGuid().ToString()
        };

        // Act
        await DummyDbContext.BulkUpdateAsync([dummyToUpdate], options: new()
        {
            PropertiesToMatch = [dummy => dummy.Id]
        });
        var dummiesAfterUpdate = await DummyDbContext.Dummies.ToArrayAsync();

        // Assert
        var dummyAfterUpdate = Assert.Single(dummiesAfterUpdate);
        Assert.Equal(dummyToUpdate.Id, dummyAfterUpdate.Id);
        Assert.Equal(dummyToUpdate.Name, dummyAfterUpdate.Name);
    }

    [Fact]
    public async Task BulkUpdateAsync_WhenPropertiesToUpdateIsEmpty_ShouldUpdateAllProperties()
    {
        // Arrange
        var existingDummy = await PreloadDummyAsync();
        var nameBeforeUpdate = existingDummy.Name;
        var dateCreatedBeforeUpdate = existingDummy.DateCreatedUtc;
        existingDummy.Name = Guid.NewGuid().ToString();
        existingDummy.DateCreatedUtc = DateTime.UtcNow.AddDays(1);

        // Act
        await DummyDbContext.BulkUpdateAsync([existingDummy], options: new()
        {
            PropertiesToMatch = []
        });

        var dummiesAfterUpdate = await DummyDbContext.Dummies.ToArrayAsync();

        // Assert
        var dummyAfterUpdate = Assert.Single(dummiesAfterUpdate);
        Assert.Equal(existingDummy.Id, dummyAfterUpdate.Id);
        Assert.Equal(existingDummy.Name, dummyAfterUpdate.Name);
        Assert.Equal(existingDummy.DateCreatedUtc, dummyAfterUpdate.DateCreatedUtc);
        Assert.NotEqual(nameBeforeUpdate, dummyAfterUpdate.Name);
        Assert.NotEqual(dateCreatedBeforeUpdate, dummyAfterUpdate.DateCreatedUtc);
    }

    [Fact]
    public async Task BulkUpdateAsync_WhenPropertiesToUpdateIsNotEmpty_ShouldUpdateOnlySpecifiedProperties()
    {
        // Arrange
        var existingDummy = await PreloadDummyAsync();
        var nameBeforeUpdate = existingDummy.Name;
        var dateCreatedBeforeUpdate = existingDummy.DateCreatedUtc;
        existingDummy.Name = Guid.NewGuid().ToString();
        existingDummy.DateCreatedUtc = DateTime.UtcNow.AddDays(1);

        // Act
        await DummyDbContext.BulkUpdateAsync([existingDummy], options: new()
        {
            PropertiesToUpdate = [dummy => dummy.Name]
        });

        var dummiesAfterUpdate = await DummyDbContext.Dummies.ToArrayAsync();

        // Assert
        var dummyAfterUpdate = Assert.Single(dummiesAfterUpdate);
        Assert.Equal(existingDummy.Id, dummyAfterUpdate.Id);
        Assert.Equal(existingDummy.Name, dummyAfterUpdate.Name);
        Assert.Equal(dateCreatedBeforeUpdate, dummyAfterUpdate.DateCreatedUtc);
        Assert.NotEqual(nameBeforeUpdate, dummyAfterUpdate.Name);
    }
}

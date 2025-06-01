using CoreSharp.EntityFramework.Bulkoperations.Extensions;
using CoreSharp.EntityFramework.Tests.Internal.Database;
using CoreSharp.EntityFramework.Tests.Internal.Database.DbContexts;
using CoreSharp.EntityFramework.Tests.Internal.Database.DbContexts.Models;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Tests.BulkOperations.Extensions;

[Collection(nameof(DummySqlServerCollection))]
public sealed class DbContextExtensionsTests_BulkInsertAsync(DummySqlServerContainer sqlContainer)
    : DummySqlServerTestsBase(sqlContainer)
{
    [Fact]
    public async Task BulkInsertAsync_WhenDbContextIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbContext? dbContext = null;
        var dummies = Array.Empty<DummyEntity>();

        // Act
        Task Action()
            => dbContext!.BulkInsertAsync(dummies);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task BulkInsertAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        Task Action()
            => DummyDbContext.BulkInsertAsync<DummyEntity>(entities: null!);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task BulkInsertAsync_WhenEntitiesIsEmpty_ShouldNotThrowException()
    {
        // Act
        var exception = await Record.ExceptionAsync(()
            => DummyDbContext.BulkInsertAsync<DummyEntity>([]));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task BulkInsertAsync_WhenOptionsIsNull_ShouldNotThrowException()
    {
        // Arrange
        var dummies = GenerateDummies(1);

        // Act
        var exception = await Record.ExceptionAsync(()
            => DummyDbContext.BulkInsertAsync(dummies, options: null!));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task BulkInsertAsync_WhenEntityTypeIsNotPartOfDbContext_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var entities = new DbContextExtensionsTests_BulkInsertAsync[]
        {
            new(null!)
        };

        // Act
        Task Action()
            => DummyDbContext.BulkInsertAsync(entities);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Action);
    }

    [Fact]
    public async Task BulkInsertAsync_WhenEntityHasNoPrimaryKey_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var dummiesToInsert = new DummyEntityWithoutPrimaryKey[]
        {
            new ()
            {
                Name = "Name1"
            }
        };

        // Act
        Task Action()
            => DummyDbContext.BulkInsertAsync(dummiesToInsert);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Action);
    }

    [Fact]
    public async Task BulkInsertAsync_WhenComputedPropertiesToInsertIsEmpty_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var dummiesToInsert = GenerateDummies(1);

        // Act
        Task Action()
            => DummyDbContext.BulkInsertAsync(dummiesToInsert, options: new()
            {
                // Id is PK, so it will be filtered out and result to exception
                PropertiesToInsert = [dummy => dummy.Id]
            });

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Action);
    }

    [Fact]
    public async Task BulkInsertAsync_WhenPropertiesToMatchIsEmptyAndEntityExist_ShouldSkipExistingByPrimaryKey()
    {
        // Arrange
        var existingDummy = await PreloadDummyAsync();

        var dummyToInsert = new DummyEntity
        {
            Id = existingDummy.Id,
            Name = Guid.NewGuid().ToString()
        };

        // Act
        await DummyDbContext.BulkInsertAsync([dummyToInsert], options: new()
        {
            PropertiesToMatch = []
        });

        var dummiesAfterInsert = await DummyDbContext.Dummies.ToArrayAsync();

        // Assert
        var dummyAfterInsert = Assert.Single(dummiesAfterInsert);
        Assert.Equal(existingDummy.Id, dummyAfterInsert.Id);
        Assert.Equal(existingDummy.Name, dummyAfterInsert.Name);
    }

    [Fact]
    public async Task BulkInsertAsync_WhenPropertiesToMatchIsNotEmptyAndEntityExist_ShouldSkipExistingByPropertiesToMatch()
    {
        // Arrange
        var existingDummy = await PreloadDummyAsync();

        var dummyToInsert = new DummyEntity
        {
            Id = existingDummy.Id,
            Name = Guid.NewGuid().ToString()
        };

        // Act
        await DummyDbContext.BulkInsertAsync([dummyToInsert], options: new()
        {
            PropertiesToMatch = [dummy => dummy.Id]
        });

        var dummiesAfterInsert = await DummyDbContext.Dummies.ToArrayAsync();

        // Assert
        var dummyAfterInsert = Assert.Single(dummiesAfterInsert);
        Assert.Equal(existingDummy.Id, dummyAfterInsert.Id);
        Assert.Equal(existingDummy.Name, dummyAfterInsert.Name);
    }

    [Fact]
    public async Task BulkInsertAsync_WhenPropertiesToInsertIsEmpty_ShouldInsertAllProperties()
    {
        // Arrange
        var dummiesToInsert = GenerateDummies(1);

        // Act
        await DummyDbContext.BulkInsertAsync(dummiesToInsert, options: new()
        {
            PropertiesToInsert = []
        });

        var dummiesAfterInsert = await DummyDbContext.Dummies.ToArrayAsync();

        // Assert
        var dummyAfterInsert = Assert.Single(dummiesAfterInsert);
        Assert.Equal(dummiesToInsert[0].Id, dummyAfterInsert.Id);
        Assert.Equal(dummiesToInsert[0].Name, dummyAfterInsert.Name);
    }

    [Fact]
    public async Task BulkInsertAsync_WhenPropertiesToInsertIsNotEmpty_ShouldInsertOnlySpecifiedProperties()
    {
        // Arrange
        var dummiesToInsert = GenerateDummies(1);

        // Act
        await DummyDbContext.BulkInsertAsync(dummiesToInsert, options: new()
        {
            PropertiesToInsert = [dummy => dummy.DateCreatedUtc]
        });

        var dummiesAfterInsert = await DummyDbContext.Dummies.ToArrayAsync();

        // Assert
        var dummyAfterInsert = Assert.Single(dummiesAfterInsert);
        Assert.Null(dummyAfterInsert.Name);
        Assert.NotNull(dummyAfterInsert.DateCreatedUtc);
    }

    [Fact]
    public async Task BulkInsertAsync_WhenEntityHasSqlValueGeneration_ShouldReturnGeneratedValue()
    {
        // Arrange
        var dummyToInsert = GenerateDummy();
        dummyToInsert.Id = Guid.Empty; // Ensure Id is not set, so it will be generated

        // Act
        await DummyDbContext.BulkInsertAsync([dummyToInsert]);

        var dummiesAfterInsert = await DummyDbContext.Dummies.ToArrayAsync();

        // Assert
        var dummyAfterInsert = Assert.Single(dummiesAfterInsert);
        Assert.NotEqual(Guid.Empty, dummyAfterInsert.Id);
    }

    [Fact]
    public async Task BulkInsertAsync_WhenEntityHasNoSqlValueGeneration_ShouldNotReturnGeneratedValue()
    {
        // Arrange
        var dummiesToInsert = new DummyWithPrimaryKeyAndNoValueGeneration[]
        {
            new ()
            {
                Name = "Name"
            }
        };

        // Act
        await DummyDbContext.BulkInsertAsync(dummiesToInsert, options: new()
        {
            PropertiesToMatch = [dummy => dummy.Name!]
        });

        var dummiesAfterInsert = await DummyDbContext.DummiesWithPrimaryKeyAndNoValueGeneration.ToArrayAsync();

        // Assert
        var dummyAfterInsert = Assert.Single(dummiesAfterInsert);
        Assert.Equal("Name", dummyAfterInsert.Name);
    }
}

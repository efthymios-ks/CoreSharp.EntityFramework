using CoreSharp.EntityFramework.Bulkoperations.Extensions;
using CoreSharp.EntityFramework.Tests.Internal.Database;
using CoreSharp.EntityFramework.Tests.Internal.Database.DbContexts;
using CoreSharp.EntityFramework.Tests.Internal.Database.DbContexts.Models;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Tests.BulkOperations.Extensions;

[Collection(nameof(DummySqlServerCollection))]
public sealed class DbContextExtensionsTests_BulkMergeAsync(DummySqlServerContainer sqlContainer)
    : DummySqlServerTestsBase(sqlContainer)
{
    [Fact]
    public async Task BulkMergeAsync_WhenDbContextIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbContext? dbContext = null;
        var dummiesToMerge = GenerateDummies(1);

        // Act
        Task Action()
            => dbContext!.BulkMergeAsync(dummiesToMerge);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task BulkMergeAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        Task Action()
            => DummyDbContext.BulkMergeAsync<DummyEntity>(entities: null!);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task BulkMergeAsync_WhenEntitiesIsEmpty_ShouldArgumentOutOfRangeException()
    {
        // Act
        Task Action()
            => DummyDbContext.BulkMergeAsync<DummyEntity>(entities: []);

        // Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(Action);
    }

    [Fact]
    public async Task BulkMergeAsync_WhenOptionsIsNull_ShouldNotThrowException()
    {
        // Arrange
        var dummiesToMerge = GenerateDummies(1);

        // Act
        var exception = await Record.ExceptionAsync(()
            => DummyDbContext.BulkMergeAsync(dummiesToMerge, options: null!));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task BulkMergeAsync_WhenEntityTypeIsNotPartOfDbContext_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var entitiesToMerge = new DbContextExtensionsTests_BulkMergeAsync[]
        {
            new(null!)
        };

        // Act
        Task Action()
            => DummyDbContext.BulkMergeAsync(entitiesToMerge);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Action);
    }

    [Fact]
    public async Task BulkMergeAsync_WhenEntityHasNoPrimaryKey_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var dummiesToInsert = new DummyEntityWithoutPrimaryKey[]
        {
            new()
            {
                Name = "Name1"
            }
        };

        // Act
        Task Action()
            => DummyDbContext.BulkMergeAsync(dummiesToInsert);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Action);
    }

    [Fact]
    public async Task BulkMergeAsync_WhenBothComputedPropertiesToInsertAndComputedPropertiesAreEmpty_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var dummiesToMge = GenerateDummies(1);

        // Act
        Task Action()
            => DummyDbContext.BulkMergeAsync(dummiesToMge, options: new()
            {
                // Id is PK, so it will be filtered out
                PropertiesToInsert = [dummy => dummy.Id],
                PropertiesToUpdate = [dummy => dummy.Id],
            });

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Action);
    }

    [Fact]
    public async Task BulkMergeAsync_WhenComputedPropertiesToInsertIsEmpty_ShouldOnlyUpdate()
    {
        // Arrange
        var existingDummy = await PreloadDummyAsync();
        var dummyToMerge = new DummyEntity
        {
            Id = existingDummy.Id,
            Name = Guid.NewGuid().ToString()
        };

        // Act
        await DummyDbContext.BulkMergeAsync([existingDummy], options: new()
        {
            // Id is PK, so it will be filtered out
            PropertiesToInsert = [dummy => dummy.Id],
            PropertiesToUpdate = []
        });

        var dummiesAfterMerge = await DummyDbContext.Dummies.ToArrayAsync();

        // Assert
        var dummyAfterMerge = Assert.Single(dummiesAfterMerge);
        Assert.Equal(existingDummy.Id, dummyAfterMerge.Id);
        Assert.Equal(existingDummy.Name, dummyAfterMerge.Name);
    }

    [Fact]
    public async Task BulkMergeAsync_WhenComputedPropertiesToUpdateIsEmpty_ShouldOnlyInsert()
    {
        // Arrange
        var dummyToMerge = GenerateDummy();
        dummyToMerge.Id = Guid.Empty; // Ensure Id is not set, so it will be generated

        // Act
        await DummyDbContext.BulkMergeAsync([dummyToMerge], options: new()
        {
            PropertiesToInsert = [],
            // Id is PK, so it will be filtered out
            PropertiesToUpdate = [dummy => dummy.Id]
        });

        var dummiesAfterMerge = await DummyDbContext.Dummies.ToArrayAsync();

        // Assert
        var dummyAfterMerge = Assert.Single(dummiesAfterMerge);
        Assert.NotEqual(Guid.Empty, dummyAfterMerge.Id);
        Assert.Equal(dummyToMerge.Name, dummyAfterMerge.Name);
    }

    [Fact]
    public async Task BulkMergeAsync_WhenPropertiesToMatchIsEmptyAndEntityExist_ShouldUpdateByPrimaryKey()
    {
        // Arrange
        var existingDummy = await PreloadDummyAsync();
        var dummyToMerge = new DummyEntity
        {
            Id = existingDummy.Id,
            Name = Guid.NewGuid().ToString()
        };

        // Act
        await DummyDbContext.BulkMergeAsync([dummyToMerge], options: new()
        {
            PropertiesToMatch = []
        });

        var dummiesAfterMerge = await DummyDbContext.Dummies.ToArrayAsync();

        // Assert
        var dummyAfterMerge = Assert.Single(dummiesAfterMerge);
        Assert.Equal(dummyToMerge.Id, dummyAfterMerge.Id);
        Assert.Equal(dummyToMerge.Name, dummyAfterMerge.Name);
    }

    [Fact]
    public async Task BulkMergeAsync_WhenPropertiesToMatchIsEmptyAndEntityDoesNotExist_ShouldInsertByPrimaryKey()
    {
        // Arrange
        var dummyToMerge = new DummyEntity
        {
            Id = Guid.NewGuid(),
            Name = Guid.NewGuid().ToString()
        };

        // Act
        await DummyDbContext.BulkMergeAsync([dummyToMerge], options: new()
        {
            PropertiesToMatch = []
        });

        var dummiesAfterMerge = await DummyDbContext.Dummies.ToArrayAsync();

        // Assert
        var dummyAfterMerge = Assert.Single(dummiesAfterMerge);
        Assert.Equal(dummyToMerge.Id, dummyAfterMerge.Id);
        Assert.Equal(dummyToMerge.Name, dummyAfterMerge.Name);
    }

    [Fact]
    public async Task BulkMergeAsync_WhenPropertiesToMatchIsEmptyAndEntityDoesNotExistAndHasSqlValueGeneration_ShouldInsertByPrimaryKeyAndGenerateValue()
    {
        // Arrange
        var dummyTomerge = GenerateDummy();
        dummyTomerge.Id = Guid.Empty; // Ensure Id is not set, so it will be generated

        // Act
        await DummyDbContext.BulkMergeAsync([dummyTomerge], options: new()
        {
            PropertiesToMatch = []
        });
        var dummiesAfterMerge = await DummyDbContext.Dummies.ToArrayAsync();

        // Assert
        var dummyAfterMerge = Assert.Single(dummiesAfterMerge);
        Assert.NotEqual(Guid.Empty, dummyAfterMerge.Id);
        Assert.Equal(dummyTomerge.Name, dummyAfterMerge.Name);
    }

    [Fact]
    public async Task BulkMergeAsync_WhenPropertiesToMatchIsNotEmptyAndEntityExist_ShouldUpdateExistingByPropertiesToMatch()
    {
        // Arrange
        var existingDummy = await PreloadDummyAsync();
        var dummyToMerge = new DummyEntity
        {
            Id = existingDummy.Id,
            Name = Guid.NewGuid().ToString()
        };

        // Act
        await DummyDbContext.BulkMergeAsync([dummyToMerge], options: new()
        {
            PropertiesToMatch = [dummy => dummy.Id]
        });

        var dummiesAfterMerge = await DummyDbContext.Dummies.ToArrayAsync();

        // Assert
        var dummyAfterMerge = Assert.Single(dummiesAfterMerge);
        Assert.Equal(dummyToMerge.Id, dummyAfterMerge.Id);
        Assert.Equal(dummyToMerge.Name, dummyAfterMerge.Name);
    }

    [Fact]
    public async Task BulkMergeAsync_WhenPropertiesToMatchIsNotEmptyAndEntityDoesNotExist_ShouldInsertByPropertiesToMatch()
    {
        // Arrange
        var dummyToMerge = new DummyEntity
        {
            Id = Guid.NewGuid(),
            Name = Guid.NewGuid().ToString()
        };

        // Act
        await DummyDbContext.BulkMergeAsync([dummyToMerge], options: new()
        {
            PropertiesToMatch = [dummy => dummy.Id]
        });

        var dummiesAfterMerge = await DummyDbContext.Dummies.ToArrayAsync();

        // Assert
        var dummyAfterMerge = Assert.Single(dummiesAfterMerge);
        Assert.Equal(dummyToMerge.Id, dummyAfterMerge.Id);
        Assert.Equal(dummyToMerge.Name, dummyAfterMerge.Name);
    }

    [Fact]
    public async Task BulkMergeAsync_WhenPropertiesToMatchIsNotEmptyAndEntityDoesNotExistAndHasSqlValueGeneration_ShouldInsertByPropertiesToMatchAndGenerateValue()
    {
        // Arrange
        var dummyToMerge = GenerateDummy();
        dummyToMerge.Id = Guid.Empty; // Ensure Id is not set, so it will be generated

        // Act
        await DummyDbContext.BulkMergeAsync([dummyToMerge], options: new()
        {
            PropertiesToMatch = [dummy => dummy.Id]
        });

        var dummiesAfterMerge = await DummyDbContext.Dummies.ToArrayAsync();

        // Assert
        var dummyAfterMerge = Assert.Single(dummiesAfterMerge);
        Assert.NotEqual(Guid.Empty, dummyAfterMerge.Id);
        Assert.Equal(dummyToMerge.Name, dummyAfterMerge.Name);
    }

    [Fact]
    public async Task BulkMergeAsync_WhenPropertiesToInsertIsEmpty_ShouldInsertAllProperties()
    {
        // Arrange
        var dummiesTomerge = GenerateDummies(1);

        // Act
        await DummyDbContext.BulkMergeAsync(dummiesTomerge, options: new()
        {
            PropertiesToInsert = []
        });

        var dummiesAfterInsert = await DummyDbContext.Dummies.ToArrayAsync();

        // Assert
        var dummyAfterInsert = Assert.Single(dummiesAfterInsert);
        Assert.Equal(dummiesTomerge[0].Id, dummyAfterInsert.Id);
        Assert.Equal(dummiesTomerge[0].Name, dummyAfterInsert.Name);
    }

    [Fact]
    public async Task BulkMergeAsync_WhenPropertiesToInsertIsNotEmpty_ShouldInsertOnlySpecifiedProperties()
    {
        // Arrange
        var dummiesToMerge = GenerateDummies(1);

        // Act
        await DummyDbContext.BulkMergeAsync(dummiesToMerge, options: new()
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
    public async Task BulkMergeAsync_WhenPropertiesToUpdateIsEmpty_ShouldUpdateAllProperties()
    {
        // Arrange
        var existingDummy = await PreloadDummyAsync();
        var dummyToMerge = new DummyEntity
        {
            Id = existingDummy.Id,
            Name = Guid.NewGuid().ToString()
        };

        // Act
        await DummyDbContext.BulkMergeAsync([dummyToMerge], options: new()
        {
            PropertiesToUpdate = []
        });

        var dummiesAfterMerge = await DummyDbContext.Dummies.ToArrayAsync();

        // Assert
        var dummyAfterMerge = Assert.Single(dummiesAfterMerge);
        Assert.Equal(existingDummy.Id, dummyAfterMerge.Id);
        Assert.Equal(dummyToMerge.Name, dummyAfterMerge.Name);
    }

    [Fact]
    public async Task BulkMergeAsync_WhenPropertiesToUpdateIsNotEmpty_ShouldUpdateOnlySpecifiedProperties()
    {
        // Arrange
        var existingDummy = await PreloadDummyAsync();
        var dummyToMerge = new DummyEntity
        {
            Id = existingDummy.Id,
            Name = Guid.NewGuid().ToString(),
            DateCreatedUtc = DateTime.UtcNow.AddDays(10) // This property should not be updated
        };

        // Act
        await DummyDbContext.BulkMergeAsync([dummyToMerge], options: new()
        {
            PropertiesToUpdate = [dummy => dummy.Name!]
        });

        var dummiesAfterMerge = await DummyDbContext.Dummies.ToArrayAsync();

        // Assert
        var dummyAfterMerge = Assert.Single(dummiesAfterMerge);
        Assert.Equal(existingDummy.Id, dummyAfterMerge.Id);
        Assert.Equal(dummyToMerge.Name, dummyAfterMerge.Name);
        Assert.Equal(existingDummy.DateCreatedUtc, dummyAfterMerge.DateCreatedUtc);
    }

    [Fact]
    public async Task BulkMergeAsync_WhenEntityHasNoSqlValueGeneration_ShouldNotReturnGeneratedValue()
    {
        // Arrange
        var dummiesToMerge = new DummyWithPrimaryKeyAndNoValueGeneration[]
        {
            new ()
            {
                Name = "Name"
            }
        };

        // Act
        await DummyDbContext.BulkMergeAsync(dummiesToMerge, options: new()
        {
            PropertiesToMatch = [dummy => dummy.Name!]
        });

        var dummiesAfterMerge = await DummyDbContext.DummiesWithPrimaryKeyAndNoValueGeneration.ToArrayAsync();

        // Assert
        var dummyAfterMerge = Assert.Single(dummiesAfterMerge);
        Assert.Equal("Name", dummyAfterMerge.Name);
    }
}

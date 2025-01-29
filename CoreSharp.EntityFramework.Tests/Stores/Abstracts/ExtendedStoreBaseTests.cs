using CoreSharp.EntityFramework.Tests.Internal.Database.Models;
using CoreSharp.EntityFramework.Tests.Internal.Database.Stores;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Tests.Stores.Abstracts;

[Collection(nameof(SharedSqlServerCollection))]
public sealed class ExtendedStoreBaseTests(SharedSqlServerContainer sqlContainer)
    : SharedSqlServerTestsBase(sqlContainer)
{
    [Fact]
    public void Constructor_WhenDbContextIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        static void Action()
            => _ = new ExtendedDummyStore(dbContext: null!);

        // Assert
        Assert.Throws<ArgumentNullException>(Action);
    }

    // AddAsync
    [Fact]
    public async Task AddAsync_WithManyEntities_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var store = new ExtendedDummyStore(DbContext);
        IEnumerable<DummyEntity> dummiesToAdd = null!;

        // Act
        Task Action()
            => store.AddAsync(dummiesToAdd);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task AddAsync_WithManyEntities_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToAdd = GenerateDummies(1);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Task Action()
            => store.AddAsync(dummiesToAdd, cancellationTokenSource.Token);

        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task AddAsync_WithManyEntities_WhenEntitiesDoNotExistInDatabase_ShouldAddEntitiesToDatabaseAndReturnThem()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToAdd = GenerateDummies(1);

        // Act
        var dummiesReturned = await store.AddAsync(dummiesToAdd);
        var dummiesRead = await DbContext.Dummies.ToArrayAsync();

        // Assert
        Assert.Equivalent(dummiesToAdd, dummiesReturned);
        Assert.Equivalent(dummiesToAdd, dummiesRead);
    }

    [Fact]
    public async Task AddAsync_WithManyEntities_WhenEntitiesExistInDatabase_ShouldThrowDbUpdateException()
    {
        // Arrange
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToAdd = await PreloadDummiesAsync(1);

        // Act
        Task Action()
            => store.AddAsync(dummiesToAdd);

        // Assert
        await Assert.ThrowsAsync<DbUpdateException>(Action);
    }

    // UpdateAsync
    [Fact]
    public async Task UpdateAsync_WithManyEntities_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var store = new ExtendedDummyStore(DbContext);
        IEnumerable<DummyEntity> dummiesToUpdate = null!;

        // Act
        Task Action()
            => store.UpdateAsync(dummiesToUpdate);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task UpdateAsync_WithManyEntities_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToUpdate = await PreloadDummiesAsync(1);
        foreach (var dummy in dummiesToUpdate)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Task Action()
            => store.UpdateAsync(dummiesToUpdate, cancellationTokenSource.Token);

        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task UpdateAsync_WithManyEntities_WhenEntitiesDoNotExistInDatabase_ShouldModifyEntitiesInDatabaseAndReturnThem()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToUpdate = GenerateDummies(1);
        foreach (var dummy in dummiesToUpdate)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        // Act
        var dummiesReturned = await store.UpdateAsync(dummiesToUpdate);
        var dummyRead = await DbContext.Dummies.ToArrayAsync();

        // Assert
        Assert.Equivalent(dummiesToUpdate, dummiesReturned);
        Assert.Equivalent(dummiesToUpdate, dummyRead);
    }

    [Fact]
    public async Task UpdateAsync_WithManyEntities_WhenEntitiesExistInDatabase_ShouldModifyEntitiesInDatabaseAndReturnThem()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToUpdate = await PreloadDummiesAsync(1);
        foreach (var dummy in dummiesToUpdate)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        // Act
        var dummiesReturned = await store.UpdateAsync(dummiesToUpdate);
        var dummyRead = await DbContext.Dummies.ToArrayAsync();

        // Assert
        Assert.Equivalent(dummiesToUpdate, dummiesReturned);
        Assert.Equivalent(dummiesToUpdate, dummyRead);
    }

    // RemoveAsync
    [Fact]
    public async Task RemoveAsync_WithManyEntities_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var store = new ExtendedDummyStore(DbContext);
        IEnumerable<DummyEntity> dummiesToRemove = null!;

        // Act
        Task Action()
            => store.RemoveAsync(dummiesToRemove);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task RemoveAsync_WithManyEntities_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToRemove = await PreloadDummiesAsync(1);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Task Action()
            => store.RemoveAsync(dummiesToRemove, cancellationTokenSource.Token);

        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task RemoveAsync_WithManyEntities_WhenEntitiesDoNotExistInDatabase_ShouldThrowDbUpdateConcurrencyException()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToRemove = GenerateDummies(1);

        // Act
        Task Action()
            => store.RemoveAsync(dummiesToRemove);

        // Assert  
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(Action);
    }

    [Fact]
    public async Task RemoveAsync_WithManyEntities_WhenEntitiesExistInDatabase_ShouldRemoveThemFromDatabase()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToRemove = await PreloadDummiesAsync(1);

        // Act
        await store.RemoveAsync(dummiesToRemove);
        var dummyRead = await DbContext.Dummies.ToArrayAsync();

        // Assert 
        Assert.Empty(dummyRead);
    }

    // RemoveByKeyAsync
    [Fact]
    public async Task RemoveAsync_ByKey_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange
        var store = new ExtendedDummyStore(DbContext);
        var existingDummy = await PreloadDummyAsync();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Task Action()
            => store.RemoveAsync(existingDummy.Id, cancellationTokenSource.Token);

        // Assert 
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task RemoveAsync_ByKey_WhenKeyExists_ShouldRemoveEntityFromDatabase()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummyToRemove = await PreloadDummyAsync();

        // Act
        await store.RemoveAsync(dummyToRemove.Id);
        var dummyEntry = await DbContext.Dummies.FindAsync(dummyToRemove.Id);

        // Assert 
        Assert.Null(dummyEntry);
    }

    [Fact]
    public async Task RemoveAsync_ByKey_WhenKeyNotFound_ShouldDoNothing()
    {
        // Arrange
        var store = new ExtendedDummyStore(DbContext);
        var dummyIdToRemove = Guid.NewGuid();

        // Act
        await store.RemoveAsync(dummyIdToRemove);
        var dummyRead = await DbContext.Dummies.FindAsync(dummyIdToRemove);

        // Assert 
        Assert.Null(dummyRead);
    }

    // ExistsAsync_WithKey
    [Fact]
    public async Task ExistsAsync_WithKey_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Task Action()
            => store.ExistsAsync(key: Guid.NewGuid(), cancellationTokenSource.Token);

        // Assert 
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task ExistsAsync_WithKey_WhenExists_ShouldReturnTrue()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var existingDummy = await PreloadDummyAsync();

        // Act
        var exists = await store.ExistsAsync(existingDummy.Id);

        // Assert 
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_WithKey_WhenDoesNotExist_ShouldReturnFalse()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);

        // Act
        var exists = await store.ExistsAsync(Guid.NewGuid());

        // Assert 
        Assert.False(exists);
    }

    // ExistsAsync_WithQuery
    [Fact]
    public async Task ExistsAsync_WithQuery_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Task Action()
            => store.ExistsAsync(cancellationToken: cancellationTokenSource.Token);

        // Assert 
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task ExistsAsync_WithQuery_WhenExists_ShouldReturnTrue()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var existingDummy = await PreloadDummyAsync();

        // Act
        var exists = await store.ExistsAsync(q => q.Where(dummy => dummy.Name == existingDummy.Name));

        // Assert 
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_WithQuery_WhenDoesNotExist_ShouldReturnFalse()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);

        // Act
        var exists = await store.ExistsAsync(q => q.Where(dummy => dummy.Name == "non-existing-name"));

        // Assert 
        Assert.False(exists);
    }

    // CountAsync
    [Fact]
    public async Task CountAsync_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Task Action()
            => store.CountAsync(cancellationToken: cancellationTokenSource.Token);

        // Assert 
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task CountAsync_WhenQueryIsProvided_ShouldReturnCountBasedOnQuery()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var existingDummies = await PreloadDummiesAsync(2);
        var dummyToFilterWith = existingDummies[0];

        // Act
        var count = await store.CountAsync(q => q.Where(dummy => dummy.Name == dummyToFilterWith.Name));

        // Assert 
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task CountAsync_WhenCalled_ShouldReturnCount()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        await PreloadDummiesAsync(2);

        // Act
        var count = await store.CountAsync();

        // Assert 
        Assert.Equal(2, count);
    }

    // AddOrUpdateAsync_Single
    [Fact]
    public async Task AddOrUpdateAsync_WithSingleEntity_WhenEntityIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var store = new ExtendedDummyStore(DbContext);
        DummyEntity dummyToAddOrUpdate = null!;

        // Act
        async Task Action()
            => await store.AddOrUpdateAsync(dummyToAddOrUpdate);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task AddOrUpdateAsync_WithSingleEntity_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummyToAddOrUpdate = GenerateDummy();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        async Task Action()
            => await store.AddOrUpdateAsync(dummyToAddOrUpdate, cancellationTokenSource.Token);

        // Assert 
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task AddOrUpdateAsync_WithSingleEntity_WhenEntityDoNotExistInDatabase_ShoulAddEntityToDatabaseAndReturnIt()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummyToAdd = GenerateDummy();

        // Act
        var dummyReturned = await store.AddOrUpdateAsync(dummyToAdd);
        var dummyRead = await DbContext.Dummies.FindAsync(dummyToAdd.Id);

        // Assert 
        Assert.Equivalent(dummyToAdd, dummyReturned);
        Assert.Equivalent(dummyToAdd, dummyRead);
    }

    [Fact]
    public async Task AddOrUpdateAsync_WithSingleEntity_WhenEntityExistsInDatabase_ShouldModifiedInDatabaseAndReturnIt()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummyToUpdate = await PreloadDummyAsync();
        dummyToUpdate.Name = Guid.NewGuid().ToString();

        // Act
        var dummyReturned = await store.AddOrUpdateAsync(dummyToUpdate);
        var dummyRead = await DbContext.Dummies.FindAsync(dummyToUpdate.Id);

        // Assert 
        Assert.Equivalent(dummyToUpdate, dummyReturned);
        Assert.Equivalent(dummyToUpdate, dummyRead);
    }

    // AddOrUpdateAsync_Many
    [Fact]
    public async Task AddOrUpdateAsync_WithManyEntities_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var store = new ExtendedDummyStore(DbContext);
        IEnumerable<DummyEntity> dummiesToAddOrUpdate = null!;

        // Act
        async Task Action()
            => await store.AddOrUpdateAsync(dummiesToAddOrUpdate);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task AddOrUpdateAsync_WithManyEntities_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToAddOrUpdate = GenerateDummies(1);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        async Task Action()
            => await store.AddOrUpdateAsync(dummiesToAddOrUpdate, cancellationTokenSource.Token);

        // Assert 
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task AddOrUpdateAsync_WithManyEntities_WhenEntitiesDoNotExistInDatabase_ShouldAddEntitiesToDatabaseAndReturnThem()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToAdd = GenerateDummies(1);

        // Act
        var dummiesReturned = await store.AddOrUpdateAsync(dummiesToAdd);
        var dummiesRead = await DbContext.Dummies.ToArrayAsync();

        // Assert 
        Assert.Equivalent(dummiesToAdd, dummiesReturned);
        Assert.Equivalent(dummiesToAdd, dummiesRead);
    }

    [Fact]
    public async Task AddOrUpdateAsync_WithManyEntities_WhenEntitiesExistInDatabase_ShouldModifyEntieisInDatabaseAndReturnThem()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToUpdate = await PreloadDummiesAsync(1);
        foreach (var dummy in dummiesToUpdate)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        // Act
        var dummiesReturned = await store.AddOrUpdateAsync(dummiesToUpdate);
        var dummiesRead = await DbContext.Dummies.ToArrayAsync();

        // Assert 
        Assert.Equivalent(dummiesToUpdate, dummiesReturned);
        Assert.Equivalent(dummiesToUpdate, dummiesRead);
    }

    [Fact]
    public async Task AddOrUpdateAsync_WithManyEntities_WhenMixedEntities_ShouldSetNewEntitiesAsAddedAndExistingEntitiesAsModifiedAndReturnThem()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToUpdate = await PreloadDummiesAsync(1);
        foreach (var dummy in dummiesToUpdate)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        var dummiesToAdd = GenerateDummies(1);
        var dummiesToAddOrUpdate = dummiesToUpdate.Concat(dummiesToAdd).ToArray();

        // Act
        var dummiesReturned = await store.AddOrUpdateAsync(dummiesToAddOrUpdate);
        var dummiesRead = await DbContext.Dummies.ToArrayAsync();

        // Assert 
        Assert.Equivalent(dummiesToAddOrUpdate, dummiesReturned);
        Assert.Equivalent(dummiesToAddOrUpdate, dummiesRead);
    }

    // AddIfNotExistAsync_Single
    [Fact]
    public async Task AddIfNotExistAsync_WithSingleEntity_WhenEntityIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var store = new ExtendedDummyStore(DbContext);
        DummyEntity dummyToAdd = null!;

        // Act
        async Task Action()
            => await store.AddIfNotExistAsync(dummyToAdd);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task AddIfNotExistAsync_WithSingleEntity_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummyToAdd = GenerateDummy();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        async Task Action()
            => await store.AddIfNotExistAsync(dummyToAdd, cancellationTokenSource.Token);

        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task AddIfNotExistAsync_WithSingleEntity_WhenEntityDoesNotExistInDatabase_ShouldAddEntityToDatabaseAndReturnIt()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummyToAdd = GenerateDummy();

        // Act
        var dummyReturned = await store.AddIfNotExistAsync(dummyToAdd);
        var dummyRead = await DbContext.Dummies.FindAsync(dummyToAdd.Id);

        // Assert
        Assert.Equivalent(dummyToAdd, dummyReturned);
        Assert.Equivalent(dummyToAdd, dummyRead);
    }

    [Fact]
    public async Task AddIfNotExistAsync_WithSingleEntity_WhenEntitysExistsInDatabase_ShouldDoNothing()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummyToAdd = await PreloadDummyAsync();

        // Act
        var dummyReturned = await store.AddIfNotExistAsync(dummyToAdd);
        var dummyRead = await DbContext.Dummies.FindAsync(dummyToAdd.Id);

        // Assert 
        Assert.Equivalent(dummyToAdd, dummyReturned);
        Assert.Equivalent(dummyToAdd, dummyRead);
    }

    // AddIfNotExistAsync_Many
    [Fact]
    public async Task AddIfNotExistAsync_WithManyEntities_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var store = new ExtendedDummyStore(DbContext);
        IEnumerable<DummyEntity> dummiesToAdd = null!;

        // Act
        async Task Action()
            => await store.AddIfNotExistAsync(dummiesToAdd);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task AddIfNotExistAsync_WithManyEntities_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToAdd = GenerateDummies(1);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        async Task Action()
            => await store.AddIfNotExistAsync(dummiesToAdd, cancellationTokenSource.Token);

        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task AddIfNotExistAsync_WithManyEntities_WhenEntitiesDoNotExistInDatabase_ShouldAddEntitiesToDatabaseAndReturnThem()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToAdd = GenerateDummies(1);

        // Act
        var dummiesReturned = await store.AddIfNotExistAsync(dummiesToAdd);
        var dummiesRead = await DbContext.Dummies.ToArrayAsync();

        // Assert
        Assert.Equivalent(dummiesToAdd, dummiesReturned);
        Assert.Equivalent(dummiesToAdd, dummiesRead);
    }

    [Fact]
    public async Task AddIfNotExistAsync_WithManyEntities_WhenEntitiesExistInDatabase_ShouldDoNothing()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToAdd = await PreloadDummiesAsync(1);

        // Act
        var dummiesReturned = await store.AddIfNotExistAsync(dummiesToAdd);
        var dummiesRead = await DbContext.Dummies.ToArrayAsync();

        // Assert 
        Assert.Empty(dummiesReturned);
        Assert.Equivalent(dummiesToAdd, dummiesRead);
    }

    // UpdateIfExistAsynct_Single
    [Fact]
    public async Task UpdateIfExistAsync_WithSingleEntity_WhenEntityIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var store = new ExtendedDummyStore(DbContext);
        DummyEntity dummyToUpdate = null!;

        // Act
        async Task Action()
            => await store.UpdateIfExistAsync(dummyToUpdate);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task UpdateIfExistAsync_WithSingleEntity_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummyToUpdate = await PreloadDummyAsync();
        dummyToUpdate.Name = Guid.NewGuid().ToString();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        async Task Action()
            => await store.UpdateIfExistAsync(dummyToUpdate, cancellationTokenSource.Token);

        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task UpdateIfExistAsync_WithSingleEntity_WhenEntityExistInDatabase_ShouldModifyEntityInDatabaseAndReturnIt()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummyToUpdate = await PreloadDummyAsync();
        dummyToUpdate.Name = Guid.NewGuid().ToString();

        // Act
        var dummyReturned = await store.UpdateIfExistAsync(dummyToUpdate);
        var dummyRead = await DbContext.Dummies.FindAsync(dummyToUpdate.Id);

        // Assert 
        Assert.Equivalent(dummyToUpdate, dummyReturned);
        Assert.Equivalent(dummyToUpdate, dummyRead);
    }

    [Fact]
    public async Task UpdateIfExistAsync_WithSingleEntity_WhenEntityDoesNotExistInDatabase_ShouldDoNothing()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummyToUpdate = GenerateDummy();

        // Act
        var dummyReturned = await store.UpdateIfExistAsync(dummyToUpdate);
        var dummyRead = await DbContext.Dummies.FindAsync(dummyToUpdate.Id);

        // Assert
        Assert.Equivalent(dummyToUpdate, dummyReturned);
        Assert.Null(dummyRead);
    }

    // UpdateIfExistAsync_Many
    [Fact]
    public async Task UpdateIfExistAsync_WithManyEntities_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var store = new ExtendedDummyStore(DbContext);
        IEnumerable<DummyEntity> dummiesToUpdate = null!;

        // Act
        async Task Action()
            => await store.UpdateIfExistAsync(dummiesToUpdate);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task UpdateIfExistAsync_WithManyEntities_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToUpdate = await PreloadDummiesAsync(1);
        foreach (var dummy in dummiesToUpdate)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        async Task Action()
            => await store.UpdateIfExistAsync(dummiesToUpdate, cancellationTokenSource.Token);

        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task UpdateIfExistAsync_WithManyEntities_WhenEntitiesExistInDatabase_ShouldModifyEntitiesInDatabaseAndReturnThem()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToUpdate = await PreloadDummiesAsync(1);
        foreach (var dummy in dummiesToUpdate)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        // Act
        var dummiesReturned = await store.UpdateIfExistAsync(dummiesToUpdate);
        var dummiesRead = await DbContext.Dummies.ToArrayAsync();

        // Assert 
        Assert.Equivalent(dummiesToUpdate, dummiesReturned);
        Assert.Equivalent(dummiesToUpdate, dummiesRead);
    }

    [Fact]
    public async Task UpdateIfExistAsync_WithManyEntities_WhenEntitiesDoNotExistInDatabase_ShouldDoNothing()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToUpdate = GenerateDummies(1);

        // Act
        var dummiesReturned = await store.UpdateIfExistAsync(dummiesToUpdate);
        var dummiesRead = await DbContext.Dummies.ToArrayAsync();

        // Assert
        Assert.Empty(dummiesReturned);
        Assert.Empty(dummiesRead);
    }

    // GetPageAsync
    [Fact]
    public async Task GetPageAsync_WhenCalled_ShouldReturnCorrectPageNumberAndSize()
    {
        // Arrange 
        const int totalItems = 5;
        const int pageNumber = 1;
        const int pageSize = 2;
        var store = new ExtendedDummyStore(DbContext);
        await PreloadDummiesAsync(totalItems);

        // Act
        var result = await store.GetPageAsync(pageNumber, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(pageNumber, result.PageNumber);
        Assert.Equal(pageSize, result.PageSize);
    }

    [Fact]
    public async Task GetPageAsync_WhenCalled_ShouldReturnCorrectTotalItems()
    {
        // Arrange 
        const int totalItems = 5;
        const int pageNumber = 1;
        const int pageSize = 2;
        var store = new ExtendedDummyStore(DbContext);
        await PreloadDummiesAsync(totalItems);

        // Act
        var result = await store.GetPageAsync(pageNumber, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(totalItems, result.TotalItems);
    }

    [Fact]
    public async Task GetPageAsync_WhenCalled_ShouldReturnCorrectTotalPages()
    {
        // Arrange 
        const int totalItems = 5;
        const int pageNumber = 1;
        const int pageSize = 2;
        var store = new ExtendedDummyStore(DbContext);
        await PreloadDummiesAsync(totalItems);

        // Act
        var result = await store.GetPageAsync(pageNumber, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.TotalPages);
    }

    [Fact]
    public async Task GetPageAsync_WhenCalled_ShouldReturnCorrectItemsInPage()
    {
        // Arrange
        const int totalItems = 5;
        const int pageNumber = 1;
        const int pageSize = 2;
        var store = new ExtendedDummyStore(DbContext);
        var existingDummies = await PreloadDummiesAsync(totalItems);
        var expectedDummies = existingDummies
            .Skip(pageNumber * pageSize)
            .Take(pageSize);

        // Act
        var result = await store.GetPageAsync(pageNumber, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(pageSize, result.Items.Count());
        Assert.Equivalent(expectedDummies, result.Items);
    }
}

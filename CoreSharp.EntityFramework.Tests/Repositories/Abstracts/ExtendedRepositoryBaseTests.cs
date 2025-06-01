using CoreSharp.EntityFramework.Tests.Internal;
using CoreSharp.EntityFramework.Tests.Internal.Database.Models;
using CoreSharp.EntityFramework.Tests.Internal.Database.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Tests.Repositories.Abstracts;

[Collection(nameof(DummySqlServerCollection))]
public sealed class ExtendedRepositoryBaseTests(DummySqlServerContainer sqlContainer)
    : DummySqlServerTestsBase(sqlContainer)
{
    [Fact]
    public void Constructor_WhenDbContextIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        static void Action()
            => _ = new ExtendedDummyRepository(dbContext: null!);

        // Assert
        Assert.Throws<ArgumentNullException>(Action);
    }

    // AddManyAsync
    [Fact]
    public async Task AddAsync_WithManyEntities_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var repository = new ExtendedDummyRepository(DummyDbContext);
        IEnumerable<DummyEntity> dummiesToAdd = null!;

        // Act
        Task Action()
            => repository.AddAsync(dummiesToAdd);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    /// <summary>
    /// Should not throw, because current implementation doesn't use it.
    /// </summary> 
    [Fact]
    public async Task AddAsync_WithManyEntities_WhenCancellationIsRequested_ShouldDoNothing()
    {
        // Arrange
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var dummiesToAdd = GenerateDummies(1);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Task Action()
            => repository.AddAsync(dummiesToAdd, cancellationTokenSource.Token);

        // Assert
        var exception = await Record.ExceptionAsync(Action);
        Assert.Null(exception);
    }

    [Fact]
    public async Task AddAsync_WithManyEntities_WhenEntitiesDoNotExistInDatabase_ShouldSetEntitiesAsAddedAndReturnThem()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var dummiesToAdd = GenerateDummies(1);

        // Act
        var dummiesReturned = await repository.AddAsync(dummiesToAdd);
        var dummyEntries = GetDummyEntries(EntityState.Added);

        // Assert
        Assert.Equivalent(dummiesToAdd, dummiesReturned);
        Assert.Equivalent(dummiesToAdd, dummyEntries.Select(entry => entry.Entity));
    }

    [Fact]
    public async Task AddAsync_WithManyEntities_WhenEntitiesExistInDatabase_ShouldSetEntitiesAsAddedAndReturnThem()
    {
        // Arrange
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var dummiesToAdd = await PreloadDummiesAsync(1);

        // Act
        var dummiesReturned = await repository.AddAsync(dummiesToAdd);
        var dummyEntries = GetDummyEntries(EntityState.Added);

        // Assert
        Assert.Equivalent(dummiesToAdd, dummiesReturned);
        Assert.Equivalent(dummiesToAdd, dummyEntries.Select(entry => entry.Entity));
    }

    // UpdateAsync
    [Fact]
    public async Task UpdateAsync_WithManyEntities_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var repository = new ExtendedDummyRepository(DummyDbContext);
        IEnumerable<DummyEntity> dummiesToUpdate = null!;

        // Act
        Task Action()
            => repository.UpdateAsync(dummiesToUpdate);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    /// <summary>
    /// Should not throw, because current implementation doesn't use it.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_WithManyEntities_WhenCancellationIsRequested_ShouldDoNothing()
    {
        // Arrange
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var dummiesToUpdate = await PreloadDummiesAsync(1);
        foreach (var dummy in dummiesToUpdate)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Task Action()
            => repository.UpdateAsync(dummiesToUpdate, cancellationTokenSource.Token);

        // Assert
        var exception = await Record.ExceptionAsync(Action);
        Assert.Null(exception);
    }

    [Fact]
    public async Task UpdateAsync_WithManyEntities_WhenEntitiesDoNotExistInDatabase_ShouldSetEntitiesAsAddedAndReturnThem()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var dummiesToUpdate = GenerateDummies(1);
        foreach (var dummy in dummiesToUpdate)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        // Act
        var dummiesReturned = await repository.UpdateAsync(dummiesToUpdate);
        var dummyEntries = GetDummyEntries(EntityState.Added);

        // Assert
        Assert.Equivalent(dummiesToUpdate, dummiesReturned);
        Assert.Equivalent(dummiesToUpdate, dummyEntries.Select(entry => entry.Entity));
    }

    [Fact]
    public async Task UpdateAsync_WithManyEntities_WhenEntitiesExistInDatabase_ShouldSetEntitiesAsModifiedAndReturnThem()
    {

        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var dummiesToUpdate = await PreloadDummiesAsync(1);
        foreach (var dummy in dummiesToUpdate)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        // Act
        var dummiesReturned = await repository.UpdateAsync(dummiesToUpdate);
        var dummyEntries = GetDummyEntries(EntityState.Modified);

        // Assert
        Assert.Equivalent(dummiesToUpdate, dummiesReturned);
        Assert.Equivalent(dummiesToUpdate, dummyEntries.Select(entry => entry.Entity));
    }

    // RemoveAsync_WithManyEntities
    [Fact]
    public async Task RemoveAsync_WithManyEntities_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var repository = new ExtendedDummyRepository(DummyDbContext);
        IEnumerable<DummyEntity> dummiesToRemove = null!;

        // Act
        Task Action()
            => repository.RemoveAsync(dummiesToRemove);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    /// <summary>
    /// Should not throw, because current implementation doesn't use it.
    /// </summary>
    [Fact]
    public async Task RemoveAsync_WithManyEntities_WhenCancellationIsRequested_ShouldDoNothing()
    {
        // Arrange
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var dummiesToRemove = await PreloadDummiesAsync(1);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Task Action()
            => repository.RemoveAsync(dummiesToRemove, cancellationTokenSource.Token);

        // Assert
        var exception = await Record.ExceptionAsync(Action);
        Assert.Null(exception);
    }

    [Fact]
    public async Task RemoveAsync_WithManyEntities_WhenEntitiesDoNotExistInDatabase_ShouldSetEntitiesAsDeletedAndReturnThem()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var dummiesToRemove = GenerateDummies(1);

        // Act
        await repository.RemoveAsync(dummiesToRemove);
        var dummyEntries = GetDummyEntries(EntityState.Deleted);

        // Assert  
        Assert.Equivalent(dummiesToRemove, dummyEntries.Select(entry => entry.Entity));
    }

    [Fact]
    public async Task RemoveAsync_WithManyEntities_WhenEntitiesExistInDatabase_ShouldSetEntitiesAsDeletedAndReturnThem()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var dummiesToRemove = await PreloadDummiesAsync(1);

        // Act
        await repository.RemoveAsync(dummiesToRemove);
        var dummyEntries = DummyDbContext.ChangeTracker.Entries<DummyEntity>();

        // Assert 
        Assert.All(dummyEntries, entry => Assert.Equal(EntityState.Deleted, entry.State));
        Assert.Equivalent(dummiesToRemove, dummyEntries.Select(entry => entry.Entity));
    }

    // RemoveAsync_ByKey
    [Fact]
    public async Task RemoveAsync_ByKey_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var existingDummy = await PreloadDummyAsync();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Task Action()
            => repository.RemoveAsync(existingDummy.Id, cancellationTokenSource.Token);

        // Assert 
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task RemoveAsync_ByKey_WhenKeyExists_ShouldSetEntityAsDeleted()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var dummyToRemove = await PreloadDummyAsync();

        // Act
        await repository.RemoveAsync(dummyToRemove.Id);
        var dummyEntry = GetDummyEntry(EntityState.Deleted)!;

        // Assert 
        Assert.Equal(EntityState.Deleted, dummyEntry.State);
        Assert.Equivalent(dummyToRemove, dummyEntry.Entity);
    }

    [Fact]
    public async Task RemoveAsync_ByKey_WhenKeyNotFound_ShouldDoNothing()
    {
        // Arrange
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var dummyIdToRemove = Guid.NewGuid();

        // Act
        await repository.RemoveAsync(dummyIdToRemove);
        var dummyEntry = GetDummyEntry(EntityState.Deleted);

        // Assert 
        Assert.Null(dummyEntry);
    }

    // ExistsAsync_WithKey
    [Fact]
    public async Task ExistsAsync_WithKey_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Task Action()
            => repository.ExistsAsync(key: Guid.NewGuid(), cancellationTokenSource.Token);

        // Assert 
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task ExistsAsync_WithKey_WhenExists_ShouldReturnTrue()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var existingDummy = await PreloadDummyAsync();

        // Act
        var exists = await repository.ExistsAsync(existingDummy.Id);

        // Assert 
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_WithKey_WhenDoesNotExist_ShouldReturnFalse()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);

        // Act
        var exists = await repository.ExistsAsync(Guid.NewGuid());

        // Assert 
        Assert.False(exists);
    }

    // ExistsAsync_WithQuery
    [Fact]
    public async Task ExistsAsync_WithQuery_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Task Action()
            => repository.ExistsAsync(cancellationToken: cancellationTokenSource.Token);

        // Assert 
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task ExistsAsync_WithQuery_WhenExists_ShouldReturnTrue()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var existingDummy = await PreloadDummyAsync();

        // Act
        var exists = await repository.ExistsAsync(q => q.Where(dummy => dummy.Name == existingDummy.Name));

        // Assert 
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_WithQuery_WhenDoesNotExist_ShouldReturnFalse()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);

        // Act
        var exists = await repository.ExistsAsync(q => q.Where(dummy => dummy.Name == "non-existing-name"));

        // Assert 
        Assert.False(exists);
    }

    // CountAsync
    [Fact]
    public async Task CountAsync_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Task Action()
            => repository.CountAsync(cancellationToken: cancellationTokenSource.Token);

        // Assert 
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task CountAsync_WhenQueryIsProvided_ShouldReturnCountBasedOnQuery()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var existingDummies = await PreloadDummiesAsync(2);
        var dummyToFilterWith = existingDummies[0];

        // Act
        var count = await repository.CountAsync(q => q.Where(dummy => dummy.Name == dummyToFilterWith.Name));

        // Assert 
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task CountAsync_WhenCalled_ShouldReturnCount()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        await PreloadDummiesAsync(2);

        // Act
        var count = await repository.CountAsync();

        // Assert 
        Assert.Equal(2, count);
    }

    // AddOrUpdateAsync_Single
    [Fact]
    public async Task AddOrUpdateAsync_WithSingleEntity_WhenEntityIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var repository = new ExtendedDummyRepository(DummyDbContext);
        DummyEntity dummyToAddOrUpdate = null!;

        // Act
        Task Action()
            => repository.AddOrUpdateAsync(dummyToAddOrUpdate);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task AddOrUpdateAsync_WithSingleEntity_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var dummyToAddOrUpdate = GenerateDummy();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Task Action()
            => repository.AddOrUpdateAsync(dummyToAddOrUpdate, cancellationTokenSource.Token);

        // Assert 
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task AddOrUpdateAsync_WithSingleEntity_WhenEntityDoNotExistInDatabase_ShouldSetEntityAsAddedAndReturnIt()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var dummyToAdd = GenerateDummy();

        // Act
        var dummyReturned = await repository.AddOrUpdateAsync(dummyToAdd);
        var dummyEntry = GetDummyEntry(EntityState.Added)!;

        // Assert 
        Assert.Equivalent(dummyToAdd, dummyReturned);
        Assert.NotNull(dummyEntry);
        Assert.Equivalent(dummyToAdd, dummyEntry.Entity);
    }

    [Fact]
    public async Task AddOrUpdateAsync_WithSingleEntity_WhenEntityExistsInDatabase_ShouldSetEntityAsModifiedAndReturnIt()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var dummyToUpdate = await PreloadDummyAsync();
        dummyToUpdate.Name = Guid.NewGuid().ToString();

        // Act
        var dummyReturned = await repository.AddOrUpdateAsync(dummyToUpdate);
        var dummyEntry = GetDummyEntry(EntityState.Modified)!;

        // Assert 
        Assert.Equivalent(dummyToUpdate, dummyReturned);
        Assert.NotNull(dummyEntry);
        Assert.Equivalent(dummyToUpdate, dummyEntry.Entity);
    }

    // AddOrUpdateAsync_Many
    [Fact]
    public async Task AddOrUpdateAsync_WithManyEntities_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var repository = new ExtendedDummyRepository(DummyDbContext);
        IEnumerable<DummyEntity> dummiesToAddOrUpdate = null!;

        // Act
        Task Action()
            => repository.AddOrUpdateAsync(dummiesToAddOrUpdate);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task AddOrUpdateAsync_WithManyEntities_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var dummiesToAddOrUpdate = GenerateDummies(1);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Task Action()
            => repository.AddOrUpdateAsync(dummiesToAddOrUpdate, cancellationTokenSource.Token);

        // Assert 
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task AddOrUpdateAsync_WithManyEntities_WhenEntitiesDoNotExistInDatabase_ShouldSetEntitiesAsAddedAndReturnThem()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var dummiesToAdd = GenerateDummies(1);

        // Act
        var dummiesReturned = await repository.AddOrUpdateAsync(dummiesToAdd);
        var dummyEntries = GetDummyEntries(EntityState.Added);

        // Assert 
        Assert.Equivalent(dummiesToAdd, dummiesReturned);
        Assert.Equivalent(dummiesToAdd, dummyEntries.Select(entry => entry.Entity));
    }

    [Fact]
    public async Task AddOrUpdateAsync_WithManyEntities_WhenEntitiesExistInDatabase_ShouldSetEntitiesAsModifiedAndReturnThem()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var dummiesToUpdate = await PreloadDummiesAsync(1);
        foreach (var dummy in dummiesToUpdate)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        // Act
        var dummiesReturned = await repository.AddOrUpdateAsync(dummiesToUpdate);
        var dummyEntries = GetDummyEntries(EntityState.Modified);

        // Assert 
        Assert.Equivalent(dummiesToUpdate, dummiesReturned);
        Assert.Equivalent(dummiesToUpdate, dummyEntries.Select(entry => entry.Entity));
    }

    [Fact]
    public async Task AddOrUpdateAsync_WithManyEntities_WhenMixedEntities_ShouldSetNewEntitiesAsAddedAndExistingEntitiesAsModifiedAndReturnThem()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var dummiesToUpdate = await PreloadDummiesAsync(1);
        foreach (var dummy in dummiesToUpdate)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        var dummiesToAdd = GenerateDummies(1);
        var dummiesToAddOrUpdate = dummiesToUpdate.Concat(dummiesToAdd).ToArray();

        // Act
        var dummiesReturned = await repository.AddOrUpdateAsync(dummiesToAddOrUpdate);
        var addedEntries = GetDummyEntries(EntityState.Added);
        var mofidiedEntries = GetDummyEntries(EntityState.Modified);

        // Assert 
        Assert.Equivalent(dummiesToAddOrUpdate, dummiesReturned);
        Assert.Equivalent(dummiesToAdd, addedEntries.Select(entry => entry.Entity));
        Assert.Equivalent(dummiesToUpdate, mofidiedEntries.Select(entry => entry.Entity));
    }

    // AddIfNotExistAsync_Single
    [Fact]
    public async Task AddIfNotExistAsync_WithSingleEntity_WhenEntityIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var repository = new ExtendedDummyRepository(DummyDbContext);
        DummyEntity dummyToAdd = null!;

        // Act
        Task Action()
            => repository.AddIfNotExistAsync(dummyToAdd);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task AddIfNotExistAsync_WithSingleEntity_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var dummyToAdd = GenerateDummy();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Task Action()
            => repository.AddIfNotExistAsync(dummyToAdd, cancellationTokenSource.Token);

        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task AddIfNotExistAsync_WithSingleEntity_WhenEntityDoesNotExistInDatabase_ShouldSetEntityAsAddedAndReturnIt()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var dummyToAdd = GenerateDummy();

        // Act
        var dummyReturned = await repository.AddIfNotExistAsync(dummyToAdd);
        var dummyEntry = GetDummyEntry(EntityState.Added)!;

        // Assert
        Assert.Equivalent(dummyToAdd, dummyReturned);
        Assert.NotNull(dummyEntry);
        Assert.Equivalent(dummyToAdd, dummyEntry.Entity);
    }

    [Fact]
    public async Task AddIfNotExistAsync_WithSingleEntity_WhenEntitysExistsInDatabase_ShouldDoNothing()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var dummyToAdd = await PreloadDummyAsync();

        // Act
        var dummyReturned = await repository.AddIfNotExistAsync(dummyToAdd);
        var dummyEntry = GetDummyEntry(EntityState.Added);

        // Assert 
        Assert.Equivalent(dummyToAdd, dummyReturned);
        Assert.Null(dummyEntry);
    }

    // AddIfNotExistAsync_Many
    [Fact]
    public async Task AddIfNotExistAsync_WithManyEntities_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var repository = new ExtendedDummyRepository(DummyDbContext);
        IEnumerable<DummyEntity> dummiesToAdd = null!;

        // Act
        Task Action()
            => repository.AddIfNotExistAsync(dummiesToAdd);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task AddIfNotExistAsync_WithManyEntities_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var dummiesToAdd = GenerateDummies(1);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Task Action()
            => repository.AddIfNotExistAsync(dummiesToAdd, cancellationTokenSource.Token);

        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task AddIfNotExistAsync_WithManyEntities_WhenEntitiesDoNotExistInDatabase_ShouldSetEntitiesAsAddedAndReturnThem()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var dummiesToAdd = GenerateDummies(1);

        // Act
        var dummiesReturned = await repository.AddIfNotExistAsync(dummiesToAdd);
        var dummyEntries = GetDummyEntries(EntityState.Added);

        // Assert
        Assert.Equivalent(dummiesToAdd, dummiesReturned);
        Assert.Equivalent(dummiesToAdd, dummyEntries.Select(entry => entry.Entity));
    }

    [Fact]
    public async Task AddIfNotExistAsync_WithManyEntities_WhenEntitiesExistInDatabase_ShouldDoNothing()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var dummiesToAdd = await PreloadDummiesAsync(1);

        // Act
        var dummiesReturned = await repository.AddIfNotExistAsync(dummiesToAdd);
        var dummyEntries = GetDummyEntries(EntityState.Added);

        // Assert 
        Assert.Empty(dummiesReturned);
        Assert.Empty(dummyEntries);
    }

    // UpdateIfExistAsynct_Single
    [Fact]
    public async Task UpdateIfExistAsync_WithSingleEntity_WhenEntityIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var repository = new ExtendedDummyRepository(DummyDbContext);
        DummyEntity dummyToUpdate = null!;

        // Act
        Task Action()
            => repository.UpdateIfExistAsync(dummyToUpdate);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task UpdateIfExistAsync_WithSingleEntity_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var dummyToUpdate = await PreloadDummyAsync();
        dummyToUpdate.Name = Guid.NewGuid().ToString();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Task Action()
            => repository.UpdateIfExistAsync(dummyToUpdate, cancellationTokenSource.Token);

        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task UpdateIfExistAsync_WithSingleEntity_WhenEntityExistInDatabase_ShouldSetEntityAsModifiedAndReturnIt()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var dummyToUpdate = await PreloadDummyAsync();
        dummyToUpdate.Name = Guid.NewGuid().ToString();

        // Act
        var dummyReturned = await repository.UpdateIfExistAsync(dummyToUpdate);
        var dummyEntry = GetDummyEntry(EntityState.Modified)!;

        // Assert 
        Assert.Equivalent(dummyToUpdate, dummyReturned);
        Assert.NotNull(dummyEntry);
        Assert.Equivalent(dummyToUpdate, dummyEntry.Entity);
    }

    [Fact]
    public async Task UpdateIfExistAsync_WithSingleEntity_WhenEntityDoNotExistInDatabase_ShouldDoNothing()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var dummyToUpdate = GenerateDummy();

        // Act
        var dummyReturned = await repository.UpdateIfExistAsync(dummyToUpdate);
        var dummyEntry = GetDummyEntry(EntityState.Modified);

        // Assert
        Assert.Equivalent(dummyToUpdate, dummyReturned);
        Assert.Null(dummyEntry);
    }

    // UpdateIfExistAsync_Many
    [Fact]
    public async Task UpdateIfExistAsync_WithManyEntities_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var repository = new ExtendedDummyRepository(DummyDbContext);
        IEnumerable<DummyEntity> dummiesToUpdate = null!;

        // Act
        Task Action()
            => repository.UpdateIfExistAsync(dummiesToUpdate);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task UpdateIfExistAsync_WithManyEntities_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var dummiesToUpdate = await PreloadDummiesAsync(1);
        foreach (var dummy in dummiesToUpdate)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Task Action()
            => repository.UpdateIfExistAsync(dummiesToUpdate, cancellationTokenSource.Token);

        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task UpdateIfExistAsync_WithManyEntities_WhenEntitiesExistInDatabase_ShouldSetEntitiesAsModifiedAndReturnThem()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var dummiesToUpdate = await PreloadDummiesAsync(1);
        foreach (var dummy in dummiesToUpdate)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        // Act
        var dummiesReturned = await repository.UpdateIfExistAsync(dummiesToUpdate);
        var dummyEntries = GetDummyEntries(EntityState.Modified);

        // Assert 
        Assert.Equivalent(dummiesToUpdate, dummiesReturned);
        Assert.Equivalent(dummiesToUpdate, dummyEntries.Select(entry => entry.Entity));
    }

    [Fact]
    public async Task UpdateIfExistAsync_WithManyEntities_WhenEntitiesDoNotExistInDatabase_ShouldDoNothing()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var dummiesToUpdate = GenerateDummies(1);

        // Act
        var dummiesReturned = await repository.UpdateIfExistAsync(dummiesToUpdate);
        var dummyEntries = GetDummyEntries(EntityState.Modified);

        // Assert
        Assert.Empty(dummiesReturned);
        Assert.Empty(dummyEntries);
    }

    // GetPageAsync
    [Fact]
    public async Task GetPageAsync_WhenCalled_ShouldReturnCorrectPageNumberAndSize()
    {
        // Arrange 
        const int totalItems = 5;
        const int pageNumber = 1;
        const int pageSize = 2;
        var repository = new ExtendedDummyRepository(DummyDbContext);
        await PreloadDummiesAsync(totalItems);

        // Act
        var result = await repository.GetPageAsync(pageNumber, pageSize);

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
        var repository = new ExtendedDummyRepository(DummyDbContext);
        await PreloadDummiesAsync(totalItems);

        // Act
        var result = await repository.GetPageAsync(pageNumber, pageSize);

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
        var repository = new ExtendedDummyRepository(DummyDbContext);
        await PreloadDummiesAsync(totalItems);

        // Act
        var result = await repository.GetPageAsync(pageNumber, pageSize);

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
        var repository = new ExtendedDummyRepository(DummyDbContext);
        var existingDummies = await PreloadDummiesAsync(totalItems);
        var expectedDummies = existingDummies
            .Skip(pageNumber * pageSize)
            .Take(pageSize);

        // Act
        var result = await repository.GetPageAsync(pageNumber, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(pageSize, result.Items.Count());
        Assert.Equivalent(expectedDummies, result.Items);
    }
}

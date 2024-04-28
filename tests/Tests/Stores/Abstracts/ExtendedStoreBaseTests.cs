using Microsoft.EntityFrameworkCore;
using Tests.Internal.Database.Models;
using Tests.Internal.Database.Stores;

namespace Tests.Repositories.Abstracts;

[TestFixture]
public sealed class ExtendedStoreBaseTests : DummyDbContextTestsBase
{
    [Test]
    public void Constructor_WhenDbContextIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        Action action = () => _ = new ExtendedDummyStore(dbContext: null);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    // AddManyAsync
    [Test]
    public async Task AddAsync_WithManyEntities_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var store = new ExtendedDummyStore(DbContext);
        IEnumerable<DummyEntity> dummiesToAdd = null;

        // Act
        Func<Task> action = () => store.AddAsync(dummiesToAdd);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddAsync_WithManyEntities_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToAdd = GenerateDummies(1);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => store.AddAsync(dummiesToAdd, cancellationTokenSource.Token);

        // Assert
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task AddAsync_WithManyEntities_WhenEntitiesDoNotExistInDatabase_ShouldAddEntitiesToDatabaseAndReturnThem()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToAdd = GenerateDummies(1);

        // Act
        var dummiesReturned = await store.AddAsync(dummiesToAdd);
        var dummiesRead = await DbContext.Dummies.ToArrayAsync();

        // Assert
        dummiesReturned.Should().BeEquivalentTo(dummiesToAdd);
        dummiesRead.Should().BeEquivalentTo(dummiesToAdd);
    }

    [Test]
    public async Task AddAsync_WithManyEntities_WhenEntitiesExistInDatabase_ShouldThrowDbUpdateException()
    {
        // Arrange
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToAdd = await PreloadDummiesAsync(1);

        // Act
        Func<Task> action = () => store.AddAsync(dummiesToAdd);

        // Assert
        await action.Should().ThrowExactlyAsync<DbUpdateException>();
    }

    // UpdateAsync
    [Test]
    public async Task UpdateAsync_WithManyEntities_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var store = new ExtendedDummyStore(DbContext);
        IEnumerable<DummyEntity> dummiesToUpdate = null;

        // Act
        Func<Task> action = () => store.UpdateAsync(dummiesToUpdate);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
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
        Func<Task> action = () => store.UpdateAsync(dummiesToUpdate, cancellationTokenSource.Token);

        // Assert
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
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
        dummiesReturned.Should().BeEquivalentTo(dummiesToUpdate);
        dummyRead.Should().BeEquivalentTo(dummiesToUpdate);
    }

    [Test]
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
        dummiesReturned.Should().BeEquivalentTo(dummiesToUpdate);
        dummyRead.Should().BeEquivalentTo(dummiesToUpdate);
    }

    // RemoveAsync
    [Test]
    public async Task RemoveAsync_WithManyEntities_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var store = new ExtendedDummyStore(DbContext);
        IEnumerable<DummyEntity> dummiesToRemove = null;

        // Act
        Func<Task> action = () => store.RemoveAsync(dummiesToRemove);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task RemoveAsync_WithManyEntities_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToRemove = await PreloadDummiesAsync(1);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => store.RemoveAsync(dummiesToRemove, cancellationTokenSource.Token);

        // Assert
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task RemoveAsync_WithManyEntities_WhenEntitiesDoNotExistInDatabase_ShouldThrowDbUpdateConcurrencyException()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToRemove = GenerateDummies(1);

        // Act
        Func<Task> action = () => store.RemoveAsync(dummiesToRemove);

        // Assert  
        await action.Should().ThrowExactlyAsync<DbUpdateConcurrencyException>();
    }

    [Test]
    public async Task RemoveAsync_WithManyEntities_WhenEntitiesExistInDatabase_ShouldRemoveThemFromDatabase()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToRemove = await PreloadDummiesAsync(1);

        // Act
        await store.RemoveAsync(dummiesToRemove);
        var dummyRead = await DbContext.Dummies.ToArrayAsync();

        // Assert 
        dummyRead.Should().BeEmpty();
    }

    // RemoveByKeyAsync
    [Test]
    public async Task RemoveAsync_ByKey_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange
        var store = new ExtendedDummyStore(DbContext);
        var existingDummy = await PreloadDummyAsync();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => store.RemoveAsync(existingDummy.Id, cancellationTokenSource.Token);

        // Assert 
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task RemoveAsync_ByKey_WhenKeyExists_ShouldRemoveEntityFromDatabase()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummyToRemove = await PreloadDummyAsync();

        // Act
        await store.RemoveAsync(dummyToRemove.Id);
        var dummyEntry = await DbContext.Dummies.FindAsync(dummyToRemove.Id);

        // Assert 
        dummyEntry.Should().BeNull();
    }

    [Test]
    public async Task RemoveAsync_ByKey_WhenKeyNotFound_ShouldDoNothing()
    {
        // Arrange
        var store = new ExtendedDummyStore(DbContext);
        var dummyIdToRemove = Guid.NewGuid();

        // Act
        await store.RemoveAsync(dummyIdToRemove);
        var dummyRead = await DbContext.Dummies.FindAsync(dummyIdToRemove);

        // Assert 
        dummyRead.Should().BeNull();
    }

    // ExistsAsync_WithKey
    [Test]
    public async Task ExistsAsync_WithKey_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => store.ExistsAsync(key: Guid.NewGuid(), cancellationTokenSource.Token);

        // Assert 
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task ExistsAsync_WithKey_WhenExists_ShouldReturnTrue()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var existingDummy = await PreloadDummyAsync();

        // Act
        var exists = await store.ExistsAsync(existingDummy.Id);

        // Assert 
        exists.Should().BeTrue();
    }

    [Test]
    public async Task ExistsAsync_WithKey_WhenDoesNotExist_ShouldReturnFalse()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);

        // Act
        var exists = await store.ExistsAsync(Guid.NewGuid());

        // Assert 
        exists.Should().BeFalse();
    }

    // ExistsAsync_WithQuery
    [Test]
    public async Task ExistsAsync_WithQuery_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => store.ExistsAsync(cancellationToken: cancellationTokenSource.Token);

        // Assert 
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task ExistsAsync_WithQuery_WhenExists_ShouldReturnTrue()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var existingDummy = await PreloadDummyAsync();

        // Act
        var exists = await store.ExistsAsync(q => q.Where(dummy => dummy.Name == existingDummy.Name));

        // Assert 
        exists.Should().BeTrue();
    }

    [Test]
    public async Task ExistsAsync_WithQuery_WhenDoesNotExist_ShouldReturnFalse()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);

        // Act
        var exists = await store.ExistsAsync(q => q.Where(dummy => dummy.Name == "non-existing-name"));

        // Assert 
        exists.Should().BeFalse();
    }

    // CountAsync
    [Test]
    public async Task CountAsyncAsync_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => store.CountAsync(cancellationToken: cancellationTokenSource.Token);

        // Assert 
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task CountAsyncAsync_WhenQueryIsProvided_ShouldReturnCountBasedOnQuery()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var existingDummies = await PreloadDummiesAsync(2);
        var dummyToFilterWith = existingDummies[0];

        // Act
        var count = await store.CountAsync(q => q.Where(dummy => dummy.Name == dummyToFilterWith.Name));

        // Assert 
        count.Should().Be(1);
    }

    [Test]
    public async Task CountAsyncAsync_WhenCalled_ShouldReturnCount()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        await PreloadDummiesAsync(2);

        // Act
        var count = await store.CountAsync();

        // Assert 
        count.Should().Be(2);
    }

    // AddOrUpdateAsync_WithSingleEntity 
    [Test]
    public async Task AddOrUpdateAsync_WithSingleEntity_WhenEntityIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var store = new ExtendedDummyStore(DbContext);
        DummyEntity dummyToAddOrUpdate = null;

        // Act
        Func<Task> action = () => store.AddOrUpdateAsync(dummyToAddOrUpdate);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddOrUpdateAsync_WithSingleEntity_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummyToAddOrUpdate = GenerateDummy();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => store.AddOrUpdateAsync(dummyToAddOrUpdate, cancellationTokenSource.Token);

        // Assert 
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task AddOrUpdateAsync_WithSingleEntity_WhenEntityDoNotExistInDatabase_ShoulAddEntityToDatabaseAndReturnIt()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummyToAdd = GenerateDummy();

        // Act
        var dummyReturned = await store.AddOrUpdateAsync(dummyToAdd);
        var dummyRead = await DbContext.Dummies.FindAsync(dummyToAdd.Id);

        // Assert 
        dummyReturned.Should().BeEquivalentTo(dummyToAdd);
        dummyRead.Should().BeEquivalentTo(dummyToAdd);
    }

    [Test]
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
        dummyReturned.Should().BeEquivalentTo(dummyToUpdate);
        dummyRead.Should().BeEquivalentTo(dummyToUpdate);
    }

    // AddOrUpdateAsync_WithManyEntities 
    [Test]
    public async Task AddOrUpdateAsync_WithManyEntities_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var store = new ExtendedDummyStore(DbContext);
        IEnumerable<DummyEntity> dummiesToAddOrUpdate = null;

        // Act
        Func<Task> action = () => store.AddOrUpdateAsync(dummiesToAddOrUpdate);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddOrUpdateAsync_WithManyEntities_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToAddOrUpdate = GenerateDummies(1);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => store.AddOrUpdateAsync(dummiesToAddOrUpdate, cancellationTokenSource.Token);

        // Assert 
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task AddOrUpdateAsync_WithManyEntities_WhenEntitiesDoNotExistInDatabase_ShouldAddEntitiesToDatabaseAndReturnThem()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToAdd = GenerateDummies(1);

        // Act
        var dummiesReturned = await store.AddOrUpdateAsync(dummiesToAdd);
        var dummiesRead = await DbContext.Dummies.ToArrayAsync();

        // Assert 
        dummiesReturned.Should().BeEquivalentTo(dummiesToAdd);
        dummiesRead.Should().BeEquivalentTo(dummiesToAdd);
    }

    [Test]
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
        dummiesReturned.Should().BeEquivalentTo(dummiesToUpdate);
        dummiesRead.Should().BeEquivalentTo(dummiesToUpdate);
    }

    [Test]
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
        dummiesReturned.Should().BeEquivalentTo(dummiesToAddOrUpdate);
        dummiesRead.Should().BeEquivalentTo(dummiesToAddOrUpdate);
    }

    // AddIfNotExistAsync_WithSingleEntity
    [Test]
    public async Task AddIfNotExistAsync_WithSingleEntity_WhenEntityIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var store = new ExtendedDummyStore(DbContext);
        DummyEntity dummyToAdd = null;

        // Act
        Func<Task> action = () => store.AddIfNotExistAsync(dummyToAdd);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddIfNotExistAsync_WithSingleEntity_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummyToAdd = GenerateDummy();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => store.AddIfNotExistAsync(dummyToAdd, cancellationTokenSource.Token);

        // Assert
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task AddIfNotExistAsync_WithSingleEntity_WhenEntityDoesNotExistInDatabase_ShouldAddEntityToDatabaseAndReturnIt()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummyToAdd = GenerateDummy();

        // Act
        var dummyReturned = await store.AddIfNotExistAsync(dummyToAdd);
        var dummyRead = await DbContext.Dummies.FindAsync(dummyToAdd.Id);

        // Assert
        dummyReturned.Should().BeEquivalentTo(dummyToAdd);
        dummyRead.Should().BeEquivalentTo(dummyToAdd);
    }

    [Test]
    public async Task AddIfNotExistAsync_WithSingleEntity_WhenEntitysExistsInDatabase_ShouldDoNothing()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummyToAdd = await PreloadDummyAsync();

        // Act
        var dummyReturned = await store.AddIfNotExistAsync(dummyToAdd);
        var dummyRead = await DbContext.Dummies.FindAsync(dummyToAdd.Id);

        // Assert 
        dummyReturned.Should().BeEquivalentTo(dummyToAdd);
        dummyRead.Should().BeEquivalentTo(dummyToAdd);
    }

    // AddIfNotExistAsync_WithManyEntities 
    [Test]
    public async Task AddIfNotExistAsync_WithManyEntities_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var store = new ExtendedDummyStore(DbContext);
        IEnumerable<DummyEntity> dummiesToAdd = null;

        // Act
        Func<Task> action = () => store.AddIfNotExistAsync(dummiesToAdd);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddIfNotExistAsync_WithManyEntities_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToAdd = GenerateDummies(1);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => store.AddIfNotExistAsync(dummiesToAdd, cancellationTokenSource.Token);

        // Assert
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task AddIfNotExistAsync_WithManyEntities_WhenEntitiesDoNotExistInDatabase_ShouldAddEntitiesToDatabaseAndReturnThem()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToAdd = GenerateDummies(1);

        // Act
        var dummiesReturned = await store.AddIfNotExistAsync(dummiesToAdd);
        var dummiesRead = await DbContext.Dummies.ToArrayAsync();

        // Assert
        dummiesReturned.Should().BeEquivalentTo(dummiesToAdd);
        dummiesRead.Should().BeEquivalentTo(dummiesToAdd);
    }

    [Test]
    public async Task AddIfNotExistAsync_WithManyEntities_WhenEntitiesExistInDatabase_ShouldDoNothing()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToAdd = await PreloadDummiesAsync(1);

        // Act
        var dummiesReturned = await store.AddIfNotExistAsync(dummiesToAdd);
        var dummiesRead = await DbContext.Dummies.ToArrayAsync();

        // Assert 
        dummiesReturned.Should().BeEmpty();
        dummiesRead.Should().BeEquivalentTo(dummiesToAdd);
    }

    // UpdateIfExistAsynct_WithSingleEntity
    [Test]
    public async Task UpdateIfExistAsync_WithSingleEntity_WhenEntityIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var store = new ExtendedDummyStore(DbContext);
        DummyEntity dummyToUpdate = null;

        // Act
        Func<Task> action = () => store.UpdateIfExistAsync(dummyToUpdate);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task UpdateIfExistAsync_WithSingleEntity_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummyToUpdate = await PreloadDummyAsync();
        dummyToUpdate.Name = Guid.NewGuid().ToString();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => store.UpdateIfExistAsync(dummyToUpdate, cancellationTokenSource.Token);

        // Assert
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
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
        dummyReturned.Should().BeEquivalentTo(dummyToUpdate);
        dummyRead.Should().BeEquivalentTo(dummyToUpdate);
    }

    [Test]
    public async Task UpdateIfExistAsync_WithSingleEntity_WhenEntityDoesNotExistInDatabase_ShouldDoNothing()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummyToUpdate = GenerateDummy();

        // Act
        var dummyReturned = await store.UpdateIfExistAsync(dummyToUpdate);
        var dummyRead = await DbContext.Dummies.FindAsync(dummyToUpdate.Id);

        // Assert
        dummyReturned.Should().BeEquivalentTo(dummyToUpdate);
        dummyRead.Should().BeNull();
    }

    // UpdateIfExistAsync_WithManyEntities 
    [Test]
    public async Task UpdateIfExistAsync_WithManyEntities_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var store = new ExtendedDummyStore(DbContext);
        IEnumerable<DummyEntity> dummiesToUpdate = null;

        // Act
        Func<Task> action = () => store.UpdateIfExistAsync(dummiesToUpdate);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
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
        Func<Task> action = () => store.UpdateIfExistAsync(dummiesToUpdate, cancellationTokenSource.Token);

        // Assert
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
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
        dummiesReturned.Should().BeEquivalentTo(dummiesToUpdate);
        dummiesRead.Should().BeEquivalentTo(dummiesToUpdate);
    }

    [Test]
    public async Task UpdateIfExistAsync_WithManyEntities_WhenEntitiesDoNotExistInDatabase_ShouldDoNothing()
    {
        // Arrange 
        var store = new ExtendedDummyStore(DbContext);
        var dummiesToUpdate = GenerateDummies(1);

        // Act
        var dummiesReturned = await store.UpdateIfExistAsync(dummiesToUpdate);
        var dummiesRead = await DbContext.Dummies.ToArrayAsync();

        // Assert
        dummiesReturned.Should().BeEmpty();
        dummiesRead.Should().BeEmpty();
    }

    // GetPageAsync
    [Test]
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
        result.Should().NotBeNull();
        result.PageNumber.Should().Be(pageNumber);
        result.PageSize.Should().Be(pageSize);
    }

    [Test]
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
        result.Should().NotBeNull();
        result.TotalItems.Should().Be(totalItems);
    }

    [Test]
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
        result.Should().NotBeNull();
        result.TotalPages.Should().Be(3);
    }

    [Test]
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
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(pageSize);
        result.Items.Should().BeEquivalentTo(expectedDummies);
    }
}

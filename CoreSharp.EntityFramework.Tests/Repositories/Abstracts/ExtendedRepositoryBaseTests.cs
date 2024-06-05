using Microsoft.EntityFrameworkCore;
using Tests.Internal.Database.Models;

namespace Tests.Repositories.Abstracts;

[TestFixture]
public sealed class ExtendedRepositoryBaseTests : DummyDbContextTestsBase
{
    [Test]
    public void Constructor_WhenDbContextIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        Action action = () => _ = new ExtendedDummyRepository(dbContext: null!);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    // AddManyAsync
    [Test]
    public async Task AddAsync_WithManyEntities_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var repository = new ExtendedDummyRepository(DbContext);
        IEnumerable<DummyEntity> dummiesToAdd = null!;

        // Act
        Func<Task> action = () => repository.AddAsync(dummiesToAdd);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test(Description = "Should not throw, because current implementation doesn't use it.")]
    public async Task AddAsync_WithManyEntities_WhenCancellationIsRequested_ShouldDoNothing()
    {
        // Arrange
        var repository = new ExtendedDummyRepository(DbContext);
        var dummiesToAdd = GenerateDummies(1);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => repository.AddAsync(dummiesToAdd, cancellationTokenSource.Token);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Test]
    public async Task AddAsync_WithManyEntities_WhenEntitiesDoNotExistInDatabase_ShouldSetEntitiesAsAddedAndReturnThem()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        var dummiesToAdd = GenerateDummies(1);

        // Act
        var dummiesReturned = await repository.AddAsync(dummiesToAdd);
        var dummyEntries = GetDummyEntries(EntityState.Added);

        // Assert
        dummiesReturned.Should().BeEquivalentTo(dummiesToAdd);
        dummyEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToAdd);
    }

    [Test]
    public async Task AddAsync_WithManyEntities_WhenEntitiesExistInDatabase_ShouldSetEntitiesAsAddedAndReturnThem()
    {
        // Arrange
        var repository = new ExtendedDummyRepository(DbContext);
        var dummiesToAdd = await PreloadDummiesAsync(1);

        // Act
        var dummiesReturned = await repository.AddAsync(dummiesToAdd);
        var dummyEntries = GetDummyEntries(EntityState.Added);

        // Assert
        dummiesReturned.Should().BeEquivalentTo(dummiesToAdd);
        dummyEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToAdd);
    }

    // UpdateAsync
    [Test]
    public async Task UpdateAsync_WithManyEntities_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var repository = new ExtendedDummyRepository(DbContext);
        IEnumerable<DummyEntity> dummiesToUpdate = null!;

        // Act
        Func<Task> action = () => repository.UpdateAsync(dummiesToUpdate);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test(Description = "Should not throw, because current implementation doesn't use it.")]
    public async Task UpdateAsync_WithManyEntities_WhenCancellationIsRequested_ShouldDoNothing()
    {
        // Arrange
        var repository = new ExtendedDummyRepository(DbContext);
        var dummiesToUpdate = await PreloadDummiesAsync(1);
        foreach (var dummy in dummiesToUpdate)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => repository.UpdateAsync(dummiesToUpdate, cancellationTokenSource.Token);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Test]
    public async Task UpdateAsync_WithManyEntities_WhenEntitiesDoNotExistInDatabase_ShouldSetEntitiesAsAddedAndReturnThem()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        var dummiesToUpdate = GenerateDummies(1);
        foreach (var dummy in dummiesToUpdate)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        // Act
        var dummiesReturned = await repository.UpdateAsync(dummiesToUpdate);
        var dummyEntries = GetDummyEntries(EntityState.Added);

        // Assert
        dummiesReturned.Should().BeEquivalentTo(dummiesToUpdate);
        dummyEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToUpdate);
    }

    [Test]
    public async Task UpdateAsync_WithManyEntities_WhenEntitiesExistInDatabase_ShouldSetEntitiesAsModifiedAndReturnThem()
    {

        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        var dummiesToUpdate = await PreloadDummiesAsync(1);
        foreach (var dummy in dummiesToUpdate)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        // Act
        var dummiesReturned = await repository.UpdateAsync(dummiesToUpdate);
        var dummyEntries = GetDummyEntries(EntityState.Modified);

        // Assert
        dummiesReturned.Should().BeEquivalentTo(dummiesToUpdate);
        dummyEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToUpdate);
    }

    // RemoveAsync_WithManyEntities
    [Test]
    public async Task RemoveAsync_WithManyEntities_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var repository = new ExtendedDummyRepository(DbContext);
        IEnumerable<DummyEntity> dummiesToRemove = null!;

        // Act
        Func<Task> action = () => repository.RemoveAsync(dummiesToRemove);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test(Description = "Should not throw, because current implementation doesn't use it.")]
    public async Task RemoveAsync_WithManyEntities_WhenCancellationIsRequested_ShouldDoNothing()
    {
        // Arrange
        var repository = new ExtendedDummyRepository(DbContext);
        var dummiesToRemove = await PreloadDummiesAsync(1);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => repository.RemoveAsync(dummiesToRemove, cancellationTokenSource.Token);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Test]
    public async Task RemoveAsync_WithManyEntities_WhenEntitiesDoNotExistInDatabase_ShouldSetEntitiesAsDeletedAndReturnThem()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        var dummiesToRemove = GenerateDummies(1);

        // Act
        await repository.RemoveAsync(dummiesToRemove);
        var dummyEntries = GetDummyEntries(EntityState.Deleted);

        // Assert  
        dummyEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToRemove);
    }

    [Test]
    public async Task RemoveAsync_WithManyEntities_WhenEntitiesExistInDatabase_ShouldSetEntitiesAsDeletedAndReturnThem()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        var dummiesToRemove = await PreloadDummiesAsync(1);

        // Act
        await repository.RemoveAsync(dummiesToRemove);
        var dummyEntries = DbContext.ChangeTracker.Entries<DummyEntity>();

        // Assert 
        dummyEntries.Should().AllSatisfy(entry => entry.State.Should().Be(EntityState.Deleted));
        dummyEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToRemove);
    }

    // RemoveAsync_ByKey
    [Test]
    public async Task RemoveAsync_ByKey_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange
        var repository = new ExtendedDummyRepository(DbContext);
        var existingDummy = await PreloadDummyAsync();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => repository.RemoveAsync(existingDummy.Id, cancellationTokenSource.Token);

        // Assert 
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task RemoveAsync_ByKey_WhenKeyExists_ShouldSetEntityAsDeleted()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        var dummyToRemove = await PreloadDummyAsync();

        // Act
        await repository.RemoveAsync(dummyToRemove.Id);
        var dummyEntry = GetDummyEntry(EntityState.Deleted)!;

        // Assert 
        dummyEntry.State.Should().Be(EntityState.Deleted);
        dummyEntry.Entity.Should().BeEquivalentTo(dummyToRemove);
    }

    [Test]
    public async Task RemoveAsync_ByKey_WhenKeyNotFound_ShouldDoNothing()
    {
        // Arrange
        var repository = new ExtendedDummyRepository(DbContext);
        var dummyIdToRemove = Guid.NewGuid();

        // Act
        await repository.RemoveAsync(dummyIdToRemove);
        var dummyEntry = GetDummyEntry(EntityState.Deleted);

        // Assert 
        dummyEntry.Should().BeNull();
    }

    // ExistsAsync_WithKey
    [Test]
    public async Task ExistsAsync_WithKey_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => repository.ExistsAsync(key: Guid.NewGuid(), cancellationTokenSource.Token);

        // Assert 
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task ExistsAsync_WithKey_WhenExists_ShouldReturnTrue()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        var existingDummy = await PreloadDummyAsync();

        // Act
        var exists = await repository.ExistsAsync(existingDummy.Id);

        // Assert 
        exists.Should().BeTrue();
    }

    [Test]
    public async Task ExistsAsync_WithKey_WhenDoesNotExist_ShouldReturnFalse()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);

        // Act
        var exists = await repository.ExistsAsync(Guid.NewGuid());

        // Assert 
        exists.Should().BeFalse();
    }

    // ExistsAsync_WithQuery
    [Test]
    public async Task ExistsAsync_WithQuery_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => repository.ExistsAsync(cancellationToken: cancellationTokenSource.Token);

        // Assert 
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task ExistsAsync_WithQuery_WhenExists_ShouldReturnTrue()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        var existingDummy = await PreloadDummyAsync();

        // Act
        var exists = await repository.ExistsAsync(q => q.Where(dummy => dummy.Name == existingDummy.Name));

        // Assert 
        exists.Should().BeTrue();
    }

    [Test]
    public async Task ExistsAsync_WithQuery_WhenDoesNotExist_ShouldReturnFalse()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);

        // Act
        var exists = await repository.ExistsAsync(q => q.Where(dummy => dummy.Name == "non-existing-name"));

        // Assert 
        exists.Should().BeFalse();
    }

    // CountAsync
    [Test]
    public async Task CountAsyncAsync_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => repository.CountAsync(cancellationToken: cancellationTokenSource.Token);

        // Assert 
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task CountAsyncAsync_WhenQueryIsProvided_ShouldReturnCountBasedOnQuery()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        var existingDummies = await PreloadDummiesAsync(2);
        var dummyToFilterWith = existingDummies[0];

        // Act
        var count = await repository.CountAsync(q => q.Where(dummy => dummy.Name == dummyToFilterWith.Name));

        // Assert 
        count.Should().Be(1);
    }

    [Test]
    public async Task CountAsyncAsync_WhenCalled_ShouldReturnCount()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        await PreloadDummiesAsync(2);

        // Act
        var count = await repository.CountAsync();

        // Assert 
        count.Should().Be(2);
    }

    // AddOrUpdateAsync_WithSingleEntity 
    [Test]
    public async Task AddOrUpdateAsync_WithSingleEntity_WhenEntityIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var repository = new ExtendedDummyRepository(DbContext);
        DummyEntity dummyToAddOrUpdate = null!;

        // Act
        Func<Task> action = () => repository.AddOrUpdateAsync(dummyToAddOrUpdate);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddOrUpdateAsync_WithSingleEntity_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        var dummyToAddOrUpdate = GenerateDummy();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => repository.AddOrUpdateAsync(dummyToAddOrUpdate, cancellationTokenSource.Token);

        // Assert 
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task AddOrUpdateAsync_WithSingleEntity_WhenEntityDoNotExistInDatabase_ShouldSetEntityAsAddedAndReturnIt()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        var dummyToAdd = GenerateDummy();

        // Act
        var dummyReturned = await repository.AddOrUpdateAsync(dummyToAdd);
        var dummyEntry = GetDummyEntry(EntityState.Added)!;

        // Assert 
        dummyReturned.Should().BeEquivalentTo(dummyToAdd);
        dummyEntry.Should().NotBeNull();
        dummyEntry.Entity.Should().BeEquivalentTo(dummyToAdd);
    }

    [Test]
    public async Task AddOrUpdateAsync_WithSingleEntity_WhenEntityExistsInDatabase_ShouldSetEntityAsModifiedAndReturnIt()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        var dummyToUpdate = await PreloadDummyAsync();
        dummyToUpdate.Name = Guid.NewGuid().ToString();

        // Act
        var dummyReturned = await repository.AddOrUpdateAsync(dummyToUpdate);
        var dummyEntry = GetDummyEntry(EntityState.Modified)!;

        // Assert 
        dummyReturned.Should().BeEquivalentTo(dummyToUpdate);
        dummyEntry.Should().NotBeNull();
        dummyEntry.Entity.Should().BeEquivalentTo(dummyToUpdate);
    }

    // AddOrUpdateAsync_WithManyEntities 
    [Test]
    public async Task AddOrUpdateAsync_WithManyEntities_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var repository = new ExtendedDummyRepository(DbContext);
        IEnumerable<DummyEntity> dummiesToAddOrUpdate = null!;

        // Act
        Func<Task> action = () => repository.AddOrUpdateAsync(dummiesToAddOrUpdate);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddOrUpdateAsync_WithManyEntities_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        var dummiesToAddOrUpdate = GenerateDummies(1);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => repository.AddOrUpdateAsync(dummiesToAddOrUpdate, cancellationTokenSource.Token);

        // Assert 
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task AddOrUpdateAsync_WithManyEntities_WhenEntitiesDoNotExistInDatabase_ShouldSetEntitiesAsAddedAndReturnThem()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        var dummiesToAdd = GenerateDummies(1);

        // Act
        var dummiesReturned = await repository.AddOrUpdateAsync(dummiesToAdd);
        var dummyEntries = GetDummyEntries(EntityState.Added);

        // Assert 
        dummiesReturned.Should().BeEquivalentTo(dummiesToAdd);
        dummyEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToAdd);
    }

    [Test]
    public async Task AddOrUpdateAsync_WithManyEntities_WhenEntitiesExistInDatabase_ShouldSetEntitiesAsModifiedAndReturnThem()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        var dummiesToUpdate = await PreloadDummiesAsync(1);
        foreach (var dummy in dummiesToUpdate)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        // Act
        var dummiesReturned = await repository.AddOrUpdateAsync(dummiesToUpdate);
        var dummyEntries = GetDummyEntries(EntityState.Modified);

        // Assert 
        dummiesReturned.Should().BeEquivalentTo(dummiesToUpdate);
        dummyEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToUpdate);
    }

    [Test]
    public async Task AddOrUpdateAsync_WithManyEntities_WhenMixedEntities_ShouldSetNewEntitiesAsAddedAndExistingEntitiesAsModifiedAndReturnThem()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
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
        dummiesReturned.Should().BeEquivalentTo(dummiesToAddOrUpdate);
        addedEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToAdd);
        mofidiedEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToUpdate);
    }

    // AddIfNotExistAsync_WithSingleEntity
    [Test]
    public async Task AddIfNotExistAsync_WithSingleEntity_WhenEntityIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var repository = new ExtendedDummyRepository(DbContext);
        DummyEntity dummyToAdd = null!;

        // Act
        Func<Task> action = () => repository.AddIfNotExistAsync(dummyToAdd);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddIfNotExistAsync_WithSingleEntity_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        var dummyToAdd = GenerateDummy();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => repository.AddIfNotExistAsync(dummyToAdd, cancellationTokenSource.Token);

        // Assert
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task AddIfNotExistAsync_WithSingleEntity_WhenEntityDoesNotExistInDatabase_ShouldSetEntityAsAddedAndReturnIt()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        var dummyToAdd = GenerateDummy();

        // Act
        var dummyReturned = await repository.AddIfNotExistAsync(dummyToAdd);
        var dummyEntry = GetDummyEntry(EntityState.Added)!;

        // Assert
        dummyReturned.Should().BeEquivalentTo(dummyToAdd);
        dummyEntry.Should().NotBeNull();
        dummyEntry.Entity.Should().BeEquivalentTo(dummyToAdd);
    }

    [Test]
    public async Task AddIfNotExistAsync_WithSingleEntity_WhenEntitysExistsInDatabase_ShouldDoNothing()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        var dummyToAdd = await PreloadDummyAsync();

        // Act
        var dummyReturned = await repository.AddIfNotExistAsync(dummyToAdd);
        var dummyEntry = GetDummyEntry(EntityState.Added);

        // Assert 
        dummyReturned.Should().BeEquivalentTo(dummyToAdd);
        dummyEntry.Should().BeNull();
    }

    // AddIfNotExistAsync_WithManyEntities 
    [Test]
    public async Task AddIfNotExistAsync_WithManyEntities_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var repository = new ExtendedDummyRepository(DbContext);
        IEnumerable<DummyEntity> dummiesToAdd = null!;

        // Act
        Func<Task> action = () => repository.AddIfNotExistAsync(dummiesToAdd);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddIfNotExistAsync_WithManyEntities_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        var dummiesToAdd = GenerateDummies(1);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => repository.AddIfNotExistAsync(dummiesToAdd, cancellationTokenSource.Token);

        // Assert
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task AddIfNotExistAsync_WithManyEntities_WhenEntitiesDoNotExistInDatabase_ShouldSetEntitiesAsAddedAndReturnThem()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        var dummiesToAdd = GenerateDummies(1);

        // Act
        var dummiesReturned = await repository.AddIfNotExistAsync(dummiesToAdd);
        var dummyEntries = GetDummyEntries(EntityState.Added);

        // Assert
        dummiesReturned.Should().BeEquivalentTo(dummiesToAdd);
        dummyEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToAdd);
    }

    [Test]
    public async Task AddIfNotExistAsync_WithManyEntities_WhenEntitiesExistInDatabase_ShouldDoNothing()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        var dummiesToAdd = await PreloadDummiesAsync(1);

        // Act
        var dummiesReturned = await repository.AddIfNotExistAsync(dummiesToAdd);
        var dummyEntries = GetDummyEntries(EntityState.Added);

        // Assert 
        dummiesReturned.Should().BeEmpty();
        dummyEntries.Should().BeEmpty();
    }

    // UpdateIfExistAsynct_WithSingleEntity
    [Test]
    public async Task UpdateIfExistAsync_WithSingleEntity_WhenEntityIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var repository = new ExtendedDummyRepository(DbContext);
        DummyEntity dummyToUpdate = null!;

        // Act
        Func<Task> action = () => repository.UpdateIfExistAsync(dummyToUpdate);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task UpdateIfExistAsync_WithSingleEntity_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        var dummyToUpdate = await PreloadDummyAsync();
        dummyToUpdate.Name = Guid.NewGuid().ToString();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => repository.UpdateIfExistAsync(dummyToUpdate, cancellationTokenSource.Token);

        // Assert
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task UpdateIfExistAsync_WithSingleEntity_WhenEntityExistInDatabase_ShouldSetEntityAsModifiedAndReturnIt()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        var dummyToUpdate = await PreloadDummyAsync();
        dummyToUpdate.Name = Guid.NewGuid().ToString();

        // Act
        var dummyReturned = await repository.UpdateIfExistAsync(dummyToUpdate);
        var dummyEntry = GetDummyEntry(EntityState.Modified)!;

        // Assert 
        dummyReturned.Should().BeEquivalentTo(dummyToUpdate);
        dummyEntry.Should().NotBeNull();
        dummyEntry.Entity.Should().BeEquivalentTo(dummyToUpdate);
    }

    [Test]
    public async Task UpdateIfExistAsync_WithSingleEntity_WhenEntityDoNotExistInDatabase_ShouldDoNothing()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        var dummyToUpdate = GenerateDummy();

        // Act
        var dummyReturned = await repository.UpdateIfExistAsync(dummyToUpdate);
        var dummyEntry = GetDummyEntry(EntityState.Modified);

        // Assert
        dummyReturned.Should().BeEquivalentTo(dummyToUpdate);
        dummyEntry.Should().BeNull();
    }

    // UpdateIfExistAsync_WithManyEntities 
    [Test]
    public async Task UpdateIfExistAsync_WithManyEntities_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var repository = new ExtendedDummyRepository(DbContext);
        IEnumerable<DummyEntity> dummiesToUpdate = null!;

        // Act
        Func<Task> action = () => repository.UpdateIfExistAsync(dummiesToUpdate);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task UpdateIfExistAsync_WithManyEntities_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        var dummiesToUpdate = await PreloadDummiesAsync(1);
        foreach (var dummy in dummiesToUpdate)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => repository.UpdateIfExistAsync(dummiesToUpdate, cancellationTokenSource.Token);

        // Assert
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task UpdateIfExistAsync_WithManyEntities_WhenEntitiesExistInDatabase_ShouldSetEntitiesAsModifiedAndReturnThem()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        var dummiesToUpdate = await PreloadDummiesAsync(1);
        foreach (var dummy in dummiesToUpdate)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        // Act
        var dummiesReturned = await repository.UpdateIfExistAsync(dummiesToUpdate);
        var dummyEntries = GetDummyEntries(EntityState.Modified);

        // Assert 
        dummiesReturned.Should().BeEquivalentTo(dummiesToUpdate);
        dummyEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToUpdate);
    }

    [Test]
    public async Task UpdateIfExistAsync_WithManyEntities_WhenEntitiesDoNotExistInDatabase_ShouldDoNothing()
    {
        // Arrange 
        var repository = new ExtendedDummyRepository(DbContext);
        var dummiesToUpdate = GenerateDummies(1);

        // Act
        var dummiesReturned = await repository.UpdateIfExistAsync(dummiesToUpdate);
        var dummyEntries = GetDummyEntries(EntityState.Modified);

        // Assert
        dummiesReturned.Should().BeEmpty();
        dummyEntries.Should().BeEmpty();
    }

    // GetPageAsync
    [Test]
    public async Task GetPageAsync_WhenCalled_ShouldReturnCorrectPageNumberAndSize()
    {
        // Arrange 
        const int totalItems = 5;
        const int pageNumber = 1;
        const int pageSize = 2;
        var repository = new ExtendedDummyRepository(DbContext);
        await PreloadDummiesAsync(totalItems);

        // Act
        var result = await repository.GetPageAsync(pageNumber, pageSize);

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
        var repository = new ExtendedDummyRepository(DbContext);
        await PreloadDummiesAsync(totalItems);

        // Act
        var result = await repository.GetPageAsync(pageNumber, pageSize);

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
        var repository = new ExtendedDummyRepository(DbContext);
        await PreloadDummiesAsync(totalItems);

        // Act
        var result = await repository.GetPageAsync(pageNumber, pageSize);

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
        var repository = new ExtendedDummyRepository(DbContext);
        var existingDummies = await PreloadDummiesAsync(totalItems);
        var expectedDummies = existingDummies
            .Skip(pageNumber * pageSize)
            .Take(pageSize);

        // Act
        var result = await repository.GetPageAsync(pageNumber, pageSize);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(pageSize);
        result.Items.Should().BeEquivalentTo(expectedDummies);
    }
}

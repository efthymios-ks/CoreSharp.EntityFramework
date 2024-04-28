using Microsoft.EntityFrameworkCore;

namespace Tests.Repositories.Abstracts;

[TestFixture]
public sealed partial class RepositoryBaseTests : DummyDbContextTestsBase
{
    [Test]
    public void Constructor_WhenDbContextIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        Action action = () => _ = new DummyRepository(dbContext: null);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    [Test]
    public async Task GetAsync_WithKey_WhenNavigationIsNull_ShouldNotThrowArgumentNullException()
    {
        // Arrange
        var repository = new DummyRepository(DbContext);

        // Act
        Func<Task> action = () => repository.GetAsync(Guid.NewGuid(), query: null);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Test]
    public async Task GetAsync_WithKey_WhenNavigationIsNotSet_ShouldReturnUnfilteredEntity()
    {
        // Arrange
        var repository = new DummyRepository(DbContext);
        var existingDummyId = (await PreloadDummyAsync()).Id;

        // Act
        var dummy = await repository.GetAsync(existingDummyId, query => query);

        // Assert
        dummy.Should().NotBeNull();
    }

    [Test]
    public async Task GetAsync_WithKey_WhenNavigationIsSet_ShouldReturnFilteredEntity()
    {
        // Arrange
        var repository = new DummyRepository(DbContext);
        var existingDummyId = (await PreloadDummyAsync()).Id;

        // Act
        var dummy = await repository.GetAsync(existingDummyId, query => query.Where(d => d.DateCreatedUtc == DateTime.MinValue));

        // Assert
        dummy.Should().BeNull();
    }

    [Test]
    public async Task GetAsync_WithKey_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange
        var repository = new DummyRepository(DbContext);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => repository.GetAsync(Guid.NewGuid(), cancellationToken: cancellationTokenSource.Token);

        // Assert
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task GetAsync_WithKey_WhenKeyNotFound_ShouldReturnNull()
    {
        // Arrange
        var repository = new DummyRepository(DbContext);
        await PreloadDummiesAsync(1);

        // Act
        var dummy = await repository.GetAsync(Guid.NewGuid());

        // Assert
        dummy.Should().BeNull();
    }

    [Test]
    public async Task GetAsync_WithKey_WhenKeyFound_ShouldReturnEntity()
    {
        // Arrange
        var repository = new DummyRepository(DbContext);
        var existingDummy = await PreloadDummyAsync();

        // Act
        var readDummy = await repository.GetAsync(existingDummy.Id);

        // Assert
        readDummy.Should().BeEquivalentTo(existingDummy);
    }

    [Test]
    public async Task GetAsync_All_WhenNavigationIsNull_ShouldNotThrowArgumentNullException()
    {
        // Arrange
        var repository = new DummyRepository(DbContext);

        // Act
        Func<Task> action = () => repository.GetAsync(query: null);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Test]
    public async Task GetAsync_All_WhenNavigationIsNotSet_ShouldReturnUnfilteredEntities()
    {
        // Arrange
        var repository = new DummyRepository(DbContext);
        var existingDummies = await PreloadDummiesAsync(1);

        // Act
        var readDummies = await repository.GetAsync(query: query => query);

        // Assert
        readDummies.Should().BeEquivalentTo(existingDummies);
    }

    [Test]
    public async Task GetAsync_All_WhenNavigationIsSet_ShouldReturnFilteredEntity()
    {
        // Arrange
        var repository = new DummyRepository(DbContext);
        await PreloadDummiesAsync(1);

        // Act
        var dummies = await repository.GetAsync(query => query.Where(d => d.DateCreatedUtc == DateTime.MinValue));

        // Assert
        dummies.Should().BeEmpty();
    }

    [Test]
    public async Task GetAsync_All_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange
        var repository = new DummyRepository(DbContext);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => repository.GetAsync(cancellationToken: cancellationTokenSource.Token);

        // Assert
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task AddAsync_WhenEntityIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var repository = new DummyRepository(DbContext);

        // Act
        Func<Task> action = () => repository.AddAsync(entity: null);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test(Description = "Should not throw, because token is not used internally by EF Core.")]
    public async Task AddAsync_WhenCancellationIsRequested_ShouldNotThrowTaskCancelledException()
    {
        // Arrange
        var repository = new DummyRepository(DbContext);
        var dummyToAdd = GenerateDummy();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => repository.AddAsync(dummyToAdd, cancellationToken: cancellationTokenSource.Token);

        // Assert
        await action.Should().NotThrowAsync<TaskCanceledException>();
    }

    [Test]
    public async Task AddAsync_WhenEntityExistsInDatabase_ShouldSetEntityAsAddedAndReturnIt()
    {
        // Arrange
        var repository = new DummyRepository(DbContext);
        var dummyToAdd = GenerateDummy();

        // Act 
        var dummyReturned = await repository.AddAsync(dummyToAdd);
        var dummyEntry = GetDummyEntry(EntityState.Added);

        // Assert
        dummyReturned.Should().BeEquivalentTo(dummyToAdd);
        dummyEntry.Should().NotBeNull();
        dummyEntry.Entity.Should().BeEquivalentTo(dummyToAdd);
    }

    [Test]
    public async Task AddAsync_WhenEntityDoesNotExistInDatabase_ShouldSetEntityAsAddedAndReturnIt()
    {
        // Arrange
        var repository = new DummyRepository(DbContext);
        var dummyToAdd = await PreloadDummyAsync();

        // Act 
        var dummyReturned = await repository.AddAsync(dummyToAdd);
        var dummyEntry = GetDummyEntry(EntityState.Added);

        // Assert
        dummyReturned.Should().BeEquivalentTo(dummyToAdd);
        dummyEntry.Should().NotBeNull();
        dummyEntry.Entity.Should().BeEquivalentTo(dummyToAdd);
    }

    [Test]
    public async Task UpdateAsync_WhenEntityIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var repository = new DummyRepository(DbContext);

        // Act
        Func<Task> action = () => repository.UpdateAsync(entity: null);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test(Description = "Should not throw, because token is not used internally by EF Core.")]
    public async Task UpdateAsync_WhenCancellationIsRequested_ShouldNotThrowTaskCancelledException()
    {
        // Arrange
        var repository = new DummyRepository(DbContext);
        var existingDummy = await PreloadDummyAsync();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => repository.UpdateAsync(existingDummy, cancellationToken: cancellationTokenSource.Token);

        // Assert
        await action.Should().NotThrowAsync<TaskCanceledException>();
    }

    [Test]
    public async Task UpdateAsync_WhenEntityExistsInDatabase_ShouldSetEntityAsModifiedAndReturnIt()
    {
        // Arrange
        var repository = new DummyRepository(DbContext);
        var dumymToUpdate = await PreloadDummyAsync();
        dumymToUpdate.Name = Guid.NewGuid().ToString();

        // Act 
        var dummyReturned = await repository.UpdateAsync(dumymToUpdate);
        var dummyEntry = GetDummyEntry(EntityState.Modified);

        // Assert
        dummyReturned.Should().BeEquivalentTo(dumymToUpdate);
        dummyEntry.Should().NotBeNull();
        dummyEntry.Entity.Should().BeEquivalentTo(dumymToUpdate);
    }

    [Test]
    public async Task UpdateAsync_WhenEntityDoesNotExistInDatabase_ShouldSetEntityAsAddedAndReturnIt()
    {
        // Arrange
        var repository = new DummyRepository(DbContext);
        var dummyToUpdate = GenerateDummy();
        dummyToUpdate.Name = Guid.NewGuid().ToString();

        // Act 
        var dummyReturned = await repository.UpdateAsync(dummyToUpdate);
        var dummyEntry = GetDummyEntry(EntityState.Added);

        // Assert
        dummyReturned.Should().BeEquivalentTo(dummyToUpdate);
        dummyEntry.Should().NotBeNull();
        dummyEntry.Entity.Should().BeEquivalentTo(dummyToUpdate);
    }

    [Test]
    public async Task RemoveAsync_WhenEntityIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var repository = new DummyRepository(DbContext);

        // Act
        Func<Task> action = () => repository.RemoveAsync(entity: null);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test(Description = "Should not throw, because token is not used internally by EF Core.")]
    public async Task RemoveAsync_WhenCancellationIsRequested_ShouldNotThrowTaskCancelledException()
    {
        // Arrange
        var repository = new DummyRepository(DbContext);
        var dummyToRemove = await PreloadDummyAsync();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => repository.RemoveAsync(dummyToRemove, cancellationToken: cancellationTokenSource.Token);

        // Assert
        await action.Should().NotThrowAsync<TaskCanceledException>();
    }

    [Test]
    public async Task RemoveAsync_WhenEntityExistsInDatabase_ShouldSetEntityAsDeleted()
    {
        // Arrange
        var repository = new DummyRepository(DbContext);
        var dummyToRemove = await PreloadDummyAsync();

        // Act 
        await repository.RemoveAsync(dummyToRemove);
        var dummyEntry = GetDummyEntry(EntityState.Deleted);

        // Assert 
        dummyEntry.Should().NotBeNull();
        dummyEntry.Entity.Should().BeEquivalentTo(dummyToRemove);
    }

    [Test]
    public async Task RemoveAsync_WhenEntityDoesNotExistInDatabase_ShouldSetEntityAsDeleted()
    {
        // Arrange
        var repository = new DummyRepository(DbContext);
        var dummyToRemove = GenerateDummy();

        // Act 
        await repository.RemoveAsync(dummyToRemove);
        var dummyEntry = GetDummyEntry(EntityState.Deleted);

        // Assert 
        dummyEntry.Should().NotBeNull();
        dummyEntry.Entity.Should().BeEquivalentTo(dummyToRemove);
    }
}

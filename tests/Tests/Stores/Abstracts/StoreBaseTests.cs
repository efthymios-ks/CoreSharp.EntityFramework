using CoreSharp.EntityFramework.Stores.Abstracts;
using Microsoft.EntityFrameworkCore;
using Tests.Internal.Database.Models;

namespace Tests.Stores.Abstracts;

[TestFixture]
public sealed class StoreBaseTests : DummyDbContextTestsBase
{
    [Test]
    public void Constructor_WhenDbContextIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        Action action = () => _ = new DummyStore(dbContext: null);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    // AddAsync
    [Test]
    public async Task AddAsync_WhenEntityIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var store = new DummyStore(DbContext);

        // Act
        Func<Task> action = () => store.AddAsync(entity: null);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddAsync_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange
        var store = new DummyStore(DbContext);
        var dummyToAdd = GenerateDummy();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => store.AddAsync(dummyToAdd, cancellationToken: cancellationTokenSource.Token);

        // Assert
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task AddAsync_WhenDoesNotExistInDatabase_ShouldAddEntityToDatabaseAndReturnIt()
    {
        // Arrange
        var store = new DummyStore(DbContext);
        var dummyToAdd = GenerateDummy();

        // Act
        var dummyReturned = await store.AddAsync(dummyToAdd);
        var dummyRead = await DbContext.Dummies.FindAsync(dummyToAdd.Id);

        // Assert
        dummyReturned.Should().BeEquivalentTo(dummyReturned);
        dummyRead.Should().BeEquivalentTo(dummyToAdd);
    }

    [Test]
    public async Task AddAsync_WhenExistsInDatabase_ShouldThrowDbUpdateException()
    {
        // Arrange
        var store = new DummyStore(DbContext);
        var dummyToAdd = await PreloadDummyAsync();

        // Act
        Func<Task> action = () => store.AddAsync(dummyToAdd);

        // Assert
        await action.Should().ThrowExactlyAsync<DbUpdateException>();
    }

    // UpdateAsync
    [Test]
    public async Task UpdateAsync_WhenEntityIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var store = new DummyStore(DbContext);

        // Act
        Func<Task> action = () => store.UpdateAsync(entity: null);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task UpdateAsync_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange
        var store = new DummyStore(DbContext);
        var dummyToUpdate = await PreloadDummyAsync();
        dummyToUpdate.Name = Guid.NewGuid().ToString();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => store.UpdateAsync(dummyToUpdate, cancellationToken: cancellationTokenSource.Token);

        // Assert
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task UpdateAsync_WhenDoesNotExistInDatabase_ShouldAddEntityToDatabaseAndReturnIt()
    {
        // Arrange
        var store = new DummyStore(DbContext);
        var dummyToUpdate = GenerateDummy();
        var dummyIdBeforeUpdate = dummyToUpdate.Id;

        // Act
        var dummyReturned = await store.UpdateAsync(dummyToUpdate);
        var dummyRead = await DbContext.Dummies.FindAsync(dummyToUpdate.Id);

        // Assert
        dummyReturned.Id.Should().NotBe(dummyIdBeforeUpdate);
        dummyReturned.Should().BeEquivalentTo(dummyReturned);
        dummyRead.Should().BeEquivalentTo(dummyToUpdate);
    }

    [Test]
    public async Task UpdateAsync_WhenExistsInDatabase_ShouldModifyEntityInDatabaseAndReturnIt()
    {
        // Arrange
        var store = new DummyStore(DbContext);
        var dummyToUpdate = await PreloadDummyAsync();
        dummyToUpdate.Name = Guid.NewGuid().ToString();

        // Act
        var dummyReturned = await store.UpdateAsync(dummyToUpdate);
        var dummyRead = await DbContext.Dummies.FindAsync(dummyToUpdate.Id);

        // Assert
        dummyReturned.Should().BeEquivalentTo(dummyReturned);
        dummyRead.Should().BeEquivalentTo(dummyToUpdate);
    }

    // RemoveAsync
    [Test]
    public async Task RemoveAsync_WhenEntityIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var store = new DummyStore(DbContext);

        // Act
        Func<Task> action = () => store.RemoveAsync(entity: null);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task RemoveAsync_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange
        var store = new DummyStore(DbContext);
        var dummyToRemove = await PreloadDummyAsync();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => store.RemoveAsync(dummyToRemove, cancellationToken: cancellationTokenSource.Token);

        // Assert
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task RemoveAsync_WhenCalled_ShouldRemoveEntityFromDatabase()
    {
        // Arrange
        var store = new DummyStore(DbContext);
        var dummyToRemove = await PreloadDummyAsync();

        // Act
        await store.RemoveAsync(dummyToRemove);
        var dummyRead = await DbContext.Dummies.FindAsync(dummyToRemove.Id);

        // Assert 
        dummyRead.Should().BeNull();
    }

    private sealed class DummyStore : StoreBase<DummyEntity, Guid>
    {
        public DummyStore(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}

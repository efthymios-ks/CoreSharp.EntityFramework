using CoreSharp.EntityFramework.Tests.Internal.Database;
using CoreSharp.EntityFramework.Tests.Internal.Database.Stores;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Tests.Stores.Abstracts;

[Collection(nameof(DummySqlServerCollection))]
public sealed class StoreBaseTests(DummySqlServerContainer sqlContainer)
    : DummySqlServerTestsBase(sqlContainer)
{
    [Fact]
    public void Constructor_WhenDbContextIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        static void Action()
            => _ = new DummyStore(dbContext: null!);

        // Assert
        Assert.Throws<ArgumentNullException>(Action);
    }

    // AddAsync
    [Fact]
    public async Task AddAsync_WhenEntityIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var store = new DummyStore(DummyDbContext);

        // Act
        Task Action()
            => store.AddAsync(entity: null!);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task AddAsync_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange
        var store = new DummyStore(DummyDbContext);
        var dummyToAdd = GenerateDummy();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Task Action()
            => store.AddAsync(dummyToAdd, cancellationToken: cancellationTokenSource.Token);

        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task AddAsync_WhenDoesNotExistInDatabase_ShouldAddEntityToDatabaseAndReturnIt()
    {
        // Arrange
        var store = new DummyStore(DummyDbContext);
        var dummyToAdd = GenerateDummy();

        // Act
        var dummyReturned = await store.AddAsync(dummyToAdd);
        var dummyRead = await DummyDbContext.Dummies.FindAsync(dummyToAdd.Id);

        // Assert
        Assert.Equivalent(dummyReturned, dummyReturned);
        Assert.Equivalent(dummyRead, dummyToAdd);
    }

    [Fact]
    public async Task AddAsync_WhenExistsInDatabase_ShouldThrowDbUpdateException()
    {
        // Arrange
        var store = new DummyStore(DummyDbContext);
        var dummyToAdd = await PreloadDummyAsync();

        // Act
        Task Action()
            => store.AddAsync(dummyToAdd);

        // Assert
        await Assert.ThrowsAsync<DbUpdateException>(Action);
    }

    // UpdateAsync
    [Fact]
    public async Task UpdateAsync_WhenEntityIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var store = new DummyStore(DummyDbContext);

        // Act
        Task Action()
            => store.UpdateAsync(entity: null!);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task UpdateAsync_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange
        var store = new DummyStore(DummyDbContext);
        var dummyToUpdate = await PreloadDummyAsync();
        dummyToUpdate.Name = Guid.NewGuid().ToString();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Task Action()
            => store.UpdateAsync(dummyToUpdate, cancellationToken: cancellationTokenSource.Token);

        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task UpdateAsync_WhenDoesNotExistInDatabase_ShouldAddEntityToDatabaseAndReturnIt()
    {
        // Arrange
        var store = new DummyStore(DummyDbContext);
        var dummyToUpdate = GenerateDummy();
        var dummyIdBeforeUpdate = dummyToUpdate.Id;

        // Act
        var dummyReturned = await store.UpdateAsync(dummyToUpdate);
        var dummyRead = await DummyDbContext.Dummies.FindAsync(dummyToUpdate.Id);

        // Assert
        Assert.NotEqual(dummyIdBeforeUpdate, dummyReturned.Id);
        Assert.Equivalent(dummyReturned, dummyReturned);
        Assert.Equivalent(dummyRead, dummyToUpdate);
    }

    [Fact]
    public async Task UpdateAsync_WhenExistsInDatabase_ShouldModifyEntityInDatabaseAndReturnIt()
    {
        // Arrange
        var store = new DummyStore(DummyDbContext);
        var dummyToUpdate = await PreloadDummyAsync();
        dummyToUpdate.Name = Guid.NewGuid().ToString();

        // Act
        var dummyReturned = await store.UpdateAsync(dummyToUpdate);
        var dummyRead = await DummyDbContext.Dummies.FindAsync(dummyToUpdate.Id);

        // Assert
        Assert.Equivalent(dummyReturned, dummyReturned);
        Assert.Equivalent(dummyRead, dummyToUpdate);
    }

    // RemoveAsync
    [Fact]
    public async Task RemoveAsync_WhenEntityIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var store = new DummyStore(DummyDbContext);

        // Act
        Task Action()
            => store.RemoveAsync(entity: null!);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task RemoveAsync_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange
        var store = new DummyStore(DummyDbContext);
        var dummyToRemove = await PreloadDummyAsync();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Task Action()
            => store.RemoveAsync(dummyToRemove, cancellationToken: cancellationTokenSource.Token);

        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task RemoveAsync_WhenCalled_ShouldRemoveEntityFromDatabase()
    {
        // Arrange
        var store = new DummyStore(DummyDbContext);
        var dummyToRemove = await PreloadDummyAsync();

        // Act
        await store.RemoveAsync(dummyToRemove);
        var dummyRead = await DummyDbContext.Dummies.FindAsync(dummyToRemove.Id);

        // Assert 
        Assert.Null(dummyRead);
    }
}

using Microsoft.EntityFrameworkCore;

namespace Tests.Repositories.Abstracts;

[TestFixture]
public sealed class UnitOfWorkBaseTests : DummyDbContextTestsBase
{
    [Test]
    public void Constructor_WhenDbContextIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange 
        DbContext dbContext = null!;

        // Act
        Action action = () => _ = new DummyUnitOfWork(dbContext);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    [Test]
    public async Task CommitAsync_WhenCalled_ShouldSaveChangesAndReturnChangeCount()
    {
        // Arrange 
        var unitOfWork = new DummyUnitOfWork(DbContext);
        var dummiesToAdd = GenerateDummies(1);
        await DbContext.Dummies.AddRangeAsync(dummiesToAdd);

        // Act
        var changeCount = await unitOfWork.CommitAsync();
        var dummiesRead = await DbContext.Dummies.ToArrayAsync();

        // Assert  
        changeCount.Should().Be(1);
        dummiesRead.Should().BeEquivalentTo(dummiesToAdd);
    }

    [Test]
    public async Task CommitAsync_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var unitOfWork = new DummyUnitOfWork(DbContext);
        var dummyToAdd = GenerateDummy();
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        await DbContext.Dummies.AddAsync(dummyToAdd);
        Func<Task> action = () => unitOfWork.CommitAsync(cancellationTokenSource.Token);

        // Assert  
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task RollbackAsync_WhenCalled_ShouldRollbackPendingChanges()
    {
        // Arrange 
        var unitOfWork = new DummyUnitOfWork(DbContext);
        var dummyToAdd = GenerateDummy();
        await DbContext.Dummies.AddAsync(dummyToAdd);

        // Act
        await unitOfWork.RollbackAsync();
        var dummiesRead = await DbContext.Dummies.ToArrayAsync();

        // Assert 
        dummiesRead.Should().BeEmpty();
    }

    [Test]
    public async Task RollbackAsync_WhenEntityDeletedAndCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var unitOfWork = new DummyUnitOfWork(DbContext);
        var dummyToRemove = await PreloadDummiesAsync(1);
        DbContext.Dummies.RemoveRange(dummyToRemove);
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act 
        Func<Task> action = () => unitOfWork.RollbackAsync(cancellationTokenSource.Token);

        // Assert  
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task DisposeAsync_WhenCalled_ShouldCallDisposeAsyncOnContext()
    {
        // Arrange 
        var unitOfWork = new DummyUnitOfWork(DbContext);

        // Act
        await unitOfWork.DisposeAsync();
        Func<Task> action = () => unitOfWork.CommitAsync();

        // Assert
        await action.Should().ThrowExactlyAsync<ObjectDisposedException>();
    }

    [Test]
    public async Task DisposeAsync_WhenAlreadyDisposed_ShouldNotThrow()
    {
        // Arrange 
        var unitOfWork = new DummyUnitOfWork(DbContext);
        await unitOfWork.DisposeAsync();

        // Act
        Func<Task> action = () => unitOfWork.DisposeAsync().AsTask();

        // Assert
        await action.Should().NotThrowAsync();
    }
}

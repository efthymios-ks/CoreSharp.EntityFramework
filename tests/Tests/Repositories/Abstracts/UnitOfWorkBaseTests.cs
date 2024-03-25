using CoreSharp.EntityFramework.Repositories.Abstracts;
using Microsoft.EntityFrameworkCore;
using Tests.Internal.Database.Models;

namespace Tests.Repositories.Abstracts;

[TestFixture]
public sealed class UnitOfWorkBaseTests : DummyDbContextTestsBase
{
    [Test]
    public void Constructor_WhenDbContextIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange 
        DbContext dbContext = null;

        // Act
        Action action = () => _ = new DummyUnitOfWork(dbContext);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    [Test]
    public async Task CommitAsync_WhenCalled_ShouldSaveChanges()
    {
        // Arrange   
        var dummyToAdd = new DummyEntity
        {
            Name = Guid.NewGuid().ToString()
        };
        var unitOfWork = new DummyUnitOfWork(DbContext);

        // Act
        await DbContext.Dummies.AddAsync(dummyToAdd);
        await unitOfWork.CommitAsync();
        var dummies = await DbContext.Dummies.ToArrayAsync();

        // Assert  
        dummies.Should().BeEquivalentTo(new[] { dummyToAdd });
    }

    [Test]
    public async Task CommitAsync_WhenCancellationTokenIsSet_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var unitOfWork = new DummyUnitOfWork(DbContext);
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        await DbContext.Dummies.AddAsync(new());
        Func<Task> func = () => unitOfWork.CommitAsync(cancellationTokenSource.Token);

        // Assert  
        await func.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task RollbackAsync_WhenCalled_ShouldRollbackPendingChanges()
    {
        // Arrange   
        var dummyToAdd = new DummyEntity();
        var unitOfWork = new DummyUnitOfWork(DbContext);

        // Act
        await DbContext.Dummies.AddAsync(dummyToAdd);
        await unitOfWork.RollbackAsync();
        var dummies = await DbContext.Dummies.ToArrayAsync();

        // Assert 
        dummies.Should().BeEmpty();
    }

    [Test]
    public async Task RollbackAsync_WhenCancellationTokenIsSetAndEntityDeleted_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var unitOfWork = new DummyUnitOfWork(DbContext);
        var existingDummies = await PreloadDummiesAsync(1);
        DbContext.Dummies.RemoveRange(existingDummies);
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act 
        Func<Task> func = () => unitOfWork.RollbackAsync(cancellationTokenSource.Token);

        // Assert  
        await func.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task DisposeAsync_ShouldCallDisposeAsyncOnContext()
    {
        // Arrange 
        var unitOfWork = new DummyUnitOfWork(DbContext);
        await unitOfWork.DisposeAsync();

        // Act
        Func<Task> func = () => unitOfWork.CommitAsync();

        // Assert
        await func.Should().ThrowExactlyAsync<ObjectDisposedException>();
    }

    private sealed class DummyUnitOfWork : UnitOfWorkBase
    {
        public DummyUnitOfWork(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
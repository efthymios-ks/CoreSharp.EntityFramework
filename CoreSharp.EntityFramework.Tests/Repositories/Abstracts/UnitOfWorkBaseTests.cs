﻿using CoreSharp.EntityFramework.Tests.Internal.Database;
using CoreSharp.EntityFramework.Tests.Internal.Database.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Tests.Repositories.Abstracts;

[Collection(nameof(DummySqlServerCollection))]
public sealed class UnitOfWorkBaseTests(DummySqlServerContainer sqlContainer)
    : DummySqlServerTestsBase(sqlContainer)
{
    [Fact]
    public void Constructor_WhenDbContextIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange 
        DbContext dbContext = null!;

        // Act
        void Action()
            => _ = new DummyUnitOfWork(dbContext);

        // Assert
        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task CommitAsync_WhenCalled_ShouldSaveChangesAndReturnChangeCount()
    {
        // Arrange 
        var unitOfWork = new DummyUnitOfWork(DummyDbContext);
        var dummiesToAdd = GenerateDummies(1);
        await DummyDbContext.Dummies.AddRangeAsync(dummiesToAdd);

        // Act
        var changeCount = await unitOfWork.CommitAsync();
        var dummiesRead = await DummyDbContext.Dummies.ToArrayAsync();

        // Assert  
        Assert.Equal(1, changeCount);
        Assert.Equivalent(dummiesToAdd, dummiesRead);
    }

    [Fact]
    public async Task CommitAsync_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var unitOfWork = new DummyUnitOfWork(DummyDbContext);
        var dummyToAdd = GenerateDummy();
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        await DummyDbContext.Dummies.AddAsync(dummyToAdd);
        Task Action()
            => unitOfWork.CommitAsync(cancellationTokenSource.Token);

        // Assert  
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task RollbackAsync_WhenCalled_ShouldRollbackPendingChanges()
    {
        // Arrange 
        var unitOfWork = new DummyUnitOfWork(DummyDbContext);
        var dummyToAdd = GenerateDummy();
        await DummyDbContext.Dummies.AddAsync(dummyToAdd);

        // Act
        await unitOfWork.RollbackAsync();
        var dummiesRead = await DummyDbContext.Dummies.ToArrayAsync();

        // Assert 
        Assert.Empty(dummiesRead);
    }

    [Fact]
    public async Task RollbackAsync_WhenEntityDeletedAndCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var unitOfWork = new DummyUnitOfWork(DummyDbContext);
        var dummyToRemove = await PreloadDummiesAsync(1);
        DummyDbContext.Dummies.RemoveRange(dummyToRemove);
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act 
        Task Action()
            => unitOfWork.RollbackAsync(cancellationTokenSource.Token);

        // Assert  
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task DisposeAsync_WhenCalled_ShouldCallDisposeAsyncOnContext()
    {
        // Arrange 
        var unitOfWork = new DummyUnitOfWork(DummyDbContext);

        // Act
        await unitOfWork.DisposeAsync();
        Task Action()
            => unitOfWork.CommitAsync();

        // Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(Action);
    }

    [Fact]
    public async Task DisposeAsync_WhenAlreadyDisposed_ShouldNotThrow()
    {
        // Arrange 
        var unitOfWork = new DummyUnitOfWork(DummyDbContext);
        await unitOfWork.DisposeAsync();

        // Act
        Task Action()
            => unitOfWork.DisposeAsync().AsTask();

        // Assert
        var exception = await Record.ExceptionAsync(Action);
        Assert.Null(exception);
    }
}

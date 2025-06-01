using CoreSharp.EntityFramework.Tests.Internal;
using CoreSharp.EntityFramework.Tests.Internal.Database.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Tests.Repositories.Abstracts;

[Collection(nameof(DummySqlServerCollection))]
public sealed partial class RepositoryBaseTests(DummySqlServerContainer sqlContainer)
    : DummySqlServerTestsBase(sqlContainer)
{
    [Fact]
    public void Constructor_WhenDbContextIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        static void Action()
            => _ = new DummyRepository(dbContext: null!);

        // Assert
        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task GetAsync_WithKey_WhenNavigationIsNull_ShouldNotThrowArgumentNullException()
    {
        // Arrange
        var repository = new DummyRepository(DummyDbContext);

        // Act
        Task Action()
            => repository.GetAsync(Guid.NewGuid(), navigation: null);

        // Assert
        var exception = await Record.ExceptionAsync(Action);
        Assert.Null(exception);
    }

    [Fact]
    public async Task GetAsync_WithKey_WhenNavigationIsNotSet_ShouldReturnUnfilteredEntity()
    {
        // Arrange
        var repository = new DummyRepository(DummyDbContext);
        var existingDummyId = (await PreloadDummyAsync()).Id;

        // Act
        var dummy = await repository.GetAsync(existingDummyId, query => query);

        // Assert
        Assert.NotNull(dummy);
    }

    [Fact]
    public async Task GetAsync_WithKey_WhenNavigationIsSet_ShouldReturnFilteredEntity()
    {
        // Arrange
        var repository = new DummyRepository(DummyDbContext);
        var existingDummyId = (await PreloadDummyAsync()).Id;

        // Act
        var dummy = await repository.GetAsync(existingDummyId, query => query.Where(d => d.DateCreatedUtc == DateTime.MinValue));

        // Assert
        Assert.Null(dummy);
    }

    [Fact]
    public async Task GetAsync_WithKey_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange
        var repository = new DummyRepository(DummyDbContext);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        async Task Action()
            => await repository.GetAsync(Guid.NewGuid(), cancellationToken: cancellationTokenSource.Token);

        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task GetAsync_WithKey_WhenKeyNotFound_ShouldReturnNull()
    {
        // Arrange
        var repository = new DummyRepository(DummyDbContext);
        await PreloadDummiesAsync(1);

        // Act
        var dummy = await repository.GetAsync(Guid.NewGuid());

        // Assert
        Assert.Null(dummy);
    }

    [Fact]
    public async Task GetAsync_WithKey_WhenKeyFound_ShouldReturnEntity()
    {
        // Arrange
        var repository = new DummyRepository(DummyDbContext);
        var existingDummy = await PreloadDummyAsync();

        // Act
        var readDummy = await repository.GetAsync(existingDummy.Id);

        // Assert
        Assert.Equivalent(existingDummy, readDummy);
    }

    [Fact]
    public async Task GetAsync_All_WhenNavigationIsNull_ShouldNotThrowArgumentNullException()
    {
        // Arrange
        var repository = new DummyRepository(DummyDbContext);

        // Act
        Task Action()
            => repository.GetAsync(navigation: null);

        // Assert
        var exception = await Record.ExceptionAsync(Action);
        Assert.Null(exception);
    }

    [Fact]
    public async Task GetAsync_All_WhenNavigationIsNotSet_ShouldReturnUnfilteredEntities()
    {
        // Arrange
        var repository = new DummyRepository(DummyDbContext);
        var existingDummies = await PreloadDummiesAsync(1);

        // Act
        var readDummies = await repository.GetAsync(navigation: query => query);

        // Assert
        Assert.Equivalent(existingDummies, readDummies);
    }

    [Fact]
    public async Task GetAsync_All_WhenNavigationIsSet_ShouldReturnFilteredEntity()
    {
        // Arrange
        var repository = new DummyRepository(DummyDbContext);
        await PreloadDummiesAsync(1);

        // Act
        var dummies = await repository.GetAsync(query => query.Where(d => d.DateCreatedUtc == DateTime.MinValue));

        // Assert
        Assert.Empty(dummies);
    }

    [Fact]
    public async Task GetAsync_All_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange
        var repository = new DummyRepository(DummyDbContext);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        async Task Action()
            => await repository.GetAsync(cancellationToken: cancellationTokenSource.Token);

        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task AddAsync_WhenEntityIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var repository = new DummyRepository(DummyDbContext);

        // Act
        async Task Action()
            => await repository.AddAsync(entity: null!);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    /// <summary>
    /// Should not throw, because token is not used internally by EF Core.
    /// </summary> 
    [Fact]
    public async Task AddAsync_WhenCancellationIsRequested_ShouldNotThrowTaskCancelledException()
    {
        // Arrange
        var repository = new DummyRepository(DummyDbContext);
        var dummyToAdd = GenerateDummy();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Task Action()
            => repository.AddAsync(dummyToAdd, cancellationToken: cancellationTokenSource.Token);

        // Assert
        var exception = await Record.ExceptionAsync(Action);
        Assert.Null(exception);
    }

    [Fact]
    public async Task AddAsync_WhenEntityExistsInDatabase_ShouldSetEntityAsAddedAndReturnIt()
    {
        // Arrange
        var repository = new DummyRepository(DummyDbContext);
        var dummyToAdd = GenerateDummy();

        // Act 
        var dummyReturned = await repository.AddAsync(dummyToAdd);
        var dummyEntry = GetDummyEntry(EntityState.Added)!;

        // Assert
        Assert.Equivalent(dummyToAdd, dummyReturned);
        Assert.NotNull(dummyEntry);
        Assert.Equivalent(dummyToAdd, dummyEntry.Entity);
    }

    [Fact]
    public async Task AddAsync_WhenEntityDoesNotExistInDatabase_ShouldSetEntityAsAddedAndReturnIt()
    {
        // Arrange
        var repository = new DummyRepository(DummyDbContext);
        var dummyToAdd = await PreloadDummyAsync();

        // Act 
        var dummyReturned = await repository.AddAsync(dummyToAdd);
        var dummyEntry = GetDummyEntry(EntityState.Added)!;

        // Assert
        Assert.Equivalent(dummyToAdd, dummyReturned);
        Assert.NotNull(dummyEntry);
        Assert.Equivalent(dummyToAdd, dummyEntry.Entity);
    }

    [Fact]
    public async Task UpdateAsync_WhenEntityIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var repository = new DummyRepository(DummyDbContext);

        // Act
        async Task Action()
            => await repository.UpdateAsync(entity: null!);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    /// <summary>
    /// Should not throw, because token is not used internally by EF Core.
    /// </summary> 
    [Fact]
    public async Task UpdateAsync_WhenCancellationIsRequested_ShouldNotThrowTaskCancelledException()
    {
        // Arrange
        var repository = new DummyRepository(DummyDbContext);
        var existingDummy = await PreloadDummyAsync();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Task Action()
            => repository.UpdateAsync(existingDummy, cancellationToken: cancellationTokenSource.Token);

        // Assert
        var exception = await Record.ExceptionAsync(Action);
        Assert.Null(exception);
    }

    [Fact]
    public async Task UpdateAsync_WhenEntityExistsInDatabase_ShouldSetEntityAsModifiedAndReturnIt()
    {
        // Arrange
        var repository = new DummyRepository(DummyDbContext);
        var dumymToUpdate = await PreloadDummyAsync();
        dumymToUpdate.Name = Guid.NewGuid().ToString();

        // Act 
        var dummyReturned = await repository.UpdateAsync(dumymToUpdate);
        var dummyEntry = GetDummyEntry(EntityState.Modified)!;

        // Assert
        Assert.Equivalent(dumymToUpdate, dummyReturned);
        Assert.NotNull(dummyEntry);
        Assert.Equivalent(dumymToUpdate, dummyEntry.Entity);
    }

    [Fact]
    public async Task UpdateAsync_WhenEntityDoesNotExistInDatabase_ShouldSetEntityAsAddedAndReturnIt()
    {
        // Arrange
        var repository = new DummyRepository(DummyDbContext);
        var dummyToUpdate = GenerateDummy();
        dummyToUpdate.Name = Guid.NewGuid().ToString();

        // Act 
        var dummyReturned = await repository.UpdateAsync(dummyToUpdate);
        var dummyEntry = GetDummyEntry(EntityState.Added)!;

        // Assert
        Assert.Equivalent(dummyToUpdate, dummyReturned);
        Assert.NotNull(dummyEntry);
        Assert.Equivalent(dummyToUpdate, dummyEntry.Entity);
    }

    [Fact]
    public async Task RemoveAsync_WhenEntityIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var repository = new DummyRepository(DummyDbContext);

        // Act
        async Task Action()
            => await repository.RemoveAsync(entity: null!);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    /// <summary>
    /// Should not throw, because token is not used internally by EF Core
    /// </summary> 
    [Fact]
    public async Task RemoveAsync_WhenCancellationIsRequested_ShouldNotThrowTaskCancelledException()
    {
        // Arrange
        var repository = new DummyRepository(DummyDbContext);
        var dummyToRemove = await PreloadDummyAsync();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Task Action()
            => repository.RemoveAsync(dummyToRemove, cancellationToken: cancellationTokenSource.Token);

        // Assert
        var exception = await Record.ExceptionAsync(Action);
        Assert.Null(exception);
    }

    [Fact]
    public async Task RemoveAsync_WhenEntityExistsInDatabase_ShouldSetEntityAsDeleted()
    {
        // Arrange
        var repository = new DummyRepository(DummyDbContext);
        var dummyToRemove = await PreloadDummyAsync();

        // Act 
        await repository.RemoveAsync(dummyToRemove);
        var dummyEntry = GetDummyEntry(EntityState.Deleted)!;

        // Assert 
        Assert.NotNull(dummyEntry);
        Assert.Equivalent(dummyToRemove, dummyEntry.Entity);
    }

    [Fact]
    public async Task RemoveAsync_WhenEntityDoesNotExistInDatabase_ShouldSetEntityAsDeleted()
    {
        // Arrange
        var repository = new DummyRepository(DummyDbContext);
        var dummyToRemove = GenerateDummy();

        // Act 
        await repository.RemoveAsync(dummyToRemove);
        var dummyEntry = GetDummyEntry(EntityState.Deleted)!;

        // Assert 
        Assert.NotNull(dummyEntry);
        Assert.Equivalent(dummyToRemove, dummyEntry.Entity);
    }
}

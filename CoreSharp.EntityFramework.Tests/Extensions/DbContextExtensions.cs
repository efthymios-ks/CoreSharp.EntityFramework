using CoreSharp.EntityFramework.Extensions;
using CoreSharp.EntityFramework.Tests.Internal.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CoreSharp.EntityFramework.Tests.Extensions;

[Collection(nameof(SharedSqlServerCollection))]
public sealed class DbContextExtensionsTests(SharedSqlServerContainer sqlContainer)
    : SharedSqlServerTestsBase(sqlContainer)
{
    [Fact]
    public async Task RollbackAsync_WhenHasNoChanges_ShouldNotThrowException()
    {
        // Act
        async Task Action()
            => await DbContext.RollbackAsync();

        // Assert
        var exception = await Record.ExceptionAsync(Action);
        Assert.Null(exception);
    }

    [Fact]
    public async Task RollbackAsync_WhenEntityAdded_ShouldRemoveAddedEntity()
    {
        // Arrange
        var dummyToAdd = GenerateDummy();
        var addedDummyCountBeforeAdding = GetAddedCount();

        // Act
        await DbContext.Dummies.AddAsync(dummyToAdd);
        var addedDummyCountAfterAdding = GetAddedCount();
        await DbContext.RollbackAsync();
        var addedDummyCountAfterRollback = GetAddedCount();

        // Assert
        Assert.Equal(addedDummyCountBeforeAdding, addedDummyCountAfterRollback);
        Assert.Equal(1, addedDummyCountAfterAdding);

        int GetAddedCount()
            => GetEntryCount(EntityState.Added);
    }

    [Fact]
    public async Task RollbackAsync_WhenEntityModified_ShouldRestoreEntityToOriginalState()
    {
        // Arrange
        var dummyBeforeModification = await PreloadDummyAsync();
        var dummyBeforeModificationAsJson = SerializeDummy(dummyBeforeModification);
        var modifiedDummyCountBeforeModify = GetModifiedCount();
        dummyBeforeModification.Name = Guid.NewGuid().ToString();
        var modifiedDummyCountAfterModify = GetModifiedCount();

        // Act
        await DbContext.RollbackAsync();
        var modifiedDummyCountAfterRollback = GetModifiedCount();
        var dummyAfterRollback = await DbContext.Dummies.FindAsync(dummyBeforeModification.Id);
        var dummyAfterRollbackAsJson = SerializeDummy(dummyAfterRollback!);

        // Assert 
        Assert.Equal(modifiedDummyCountBeforeModify, modifiedDummyCountAfterRollback);
        Assert.Equal(1, modifiedDummyCountAfterModify);
        Assert.Equivalent(dummyBeforeModificationAsJson, dummyAfterRollbackAsJson);

        static string SerializeDummy(DummyEntity dummy)
            => JsonSerializer.Serialize(dummy);

        int GetModifiedCount()
            => GetEntryCount(EntityState.Modified);
    }

    [Fact]
    public async Task RollbackAsync_WhenEntityDeletedAndCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange
        var dummyToRemove = await PreloadDummyAsync();
        DbContext.Dummies.Remove(dummyToRemove);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act  
        async Task Action()
            => await DbContext.RollbackAsync(cancellationTokenSource.Token);

        // Assert 
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task RollbackAsync_WhenEntityDeleted_ShouldRestoreEntityToOriginalState()
    {
        // Arrange
        var dummyToRemove = await PreloadDummyAsync();
        var deletedDummyCountBeforeDelete = GetDeletedCount();

        // Act
        DbContext.Dummies.Remove(dummyToRemove);
        var deletedDummyCountAfterDelete = GetDeletedCount();
        await DbContext.RollbackAsync();
        var deletedDummyCountAfterRollback = GetDeletedCount();

        // Assert
        Assert.Equal(deletedDummyCountBeforeDelete, deletedDummyCountAfterRollback);
        Assert.Equal(1, deletedDummyCountAfterDelete);

        int GetDeletedCount()
            => GetEntryCount(EntityState.Deleted);
    }

    private int GetEntryCount(EntityState entityState)
        => GetDummyEntries(entityState).Length;
}

using CoreSharp.EntityFramework.Extensions;
using CoreSharp.EntityFramework.Tests.Internal.Database;
using CoreSharp.EntityFramework.Tests.Internal.Database.DbContexts;
using CoreSharp.EntityFramework.Tests.Internal.Database.DbContexts.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CoreSharp.EntityFramework.Tests.Extensions;

[Collection(nameof(DummySqlServerCollection))]
public sealed class DbContextExtensionsTests(DummySqlServerContainer sqlContainer)
    : DummySqlServerTestsBase(sqlContainer)
{
    [Fact]
    public async Task RollbackAsync_WhenHasNoChanges_ShouldNotThrowException()
    {
        // Act
        async Task Action()
            => await DummyDbContext.RollbackAsync();

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
        await DummyDbContext.Dummies.AddAsync(dummyToAdd);
        var addedDummyCountAfterAdding = GetAddedCount();
        await DummyDbContext.RollbackAsync();
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
        await DummyDbContext.RollbackAsync();
        var modifiedDummyCountAfterRollback = GetModifiedCount();
        var dummyAfterRollback = await DummyDbContext.Dummies.FindAsync(dummyBeforeModification.Id);
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
        DummyDbContext.Dummies.Remove(dummyToRemove);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act  
        async Task Action()
            => await DummyDbContext.RollbackAsync(cancellationTokenSource.Token);

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
        DummyDbContext.Dummies.Remove(dummyToRemove);
        var deletedDummyCountAfterDelete = GetDeletedCount();
        await DummyDbContext.RollbackAsync();
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

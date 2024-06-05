using CoreSharp.EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Tests.Internal.Database.Models;

namespace Tests.Extensions;

[TestFixture]
public sealed class DbContextExtensionsTests : DummyDbContextTestsBase
{
    [Test]
    public async Task RollbackAsync_WhenHasNoChanges_ShouldNotThrowException()
    {
        // Act
        Func<Task> action = () => DbContext.RollbackAsync();

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Test]
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
        addedDummyCountAfterRollback.Should().Be(addedDummyCountBeforeAdding);
        addedDummyCountAfterAdding.Should().Be(1);

        static int GetAddedCount()
            => GetEntryCount(EntityState.Added);
    }

    [Test]
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
        modifiedDummyCountAfterRollback.Should().Be(modifiedDummyCountBeforeModify);
        modifiedDummyCountAfterModify.Should().Be(1);
        dummyAfterRollbackAsJson.Should().Be(dummyBeforeModificationAsJson);

        static string SerializeDummy(DummyEntity dummy)
            => JsonSerializer.Serialize(dummy);

        static int GetModifiedCount()
            => GetEntryCount(EntityState.Modified);
    }

    [Test]
    public async Task RollbackAsync_WhenEntityDeletedAndCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange
        var dummyToRemove = await PreloadDummyAsync();
        DbContext.Dummies.Remove(dummyToRemove);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act  
        Func<Task> action = () => DbContext.RollbackAsync(cancellationTokenSource.Token);

        // Assert 
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
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
        deletedDummyCountAfterRollback.Should().Be(deletedDummyCountBeforeDelete);
        deletedDummyCountAfterDelete.Should().Be(1);

        static int GetDeletedCount()
            => GetEntryCount(EntityState.Deleted);

    }

    private static int GetEntryCount(EntityState entityState)
        => GetDummyEntries(entityState).Length;
}

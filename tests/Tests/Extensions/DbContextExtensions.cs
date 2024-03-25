using CoreSharp.EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Tests.Extensions;

[TestFixture]
public sealed class DbContextExtensionsTests : DummyDbContextTestsBase
{
    [Test]
    public async Task RollbackAsync_WhenNoChanges_ShouldNotThrowException()
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
        var dummy = GenerateDummy();
        var initialCount = await DbContext.Dummies.CountAsync();

        // Act
        await DbContext.Dummies.AddAsync(dummy);
        await DbContext.RollbackAsync();
        var finalCount = await DbContext.Dummies.CountAsync();

        // Assert
        finalCount.Should().Be(initialCount);
    }

    [Test]
    public async Task RollbackAsync_WhenEntityModified_ShouldRestoreEntityToOriginalState()
    {
        // Arrange
        var existingDummy = (await PreloadDummiesAsync(1))[0];
        var originalName = existingDummy.Name;

        // Act
        existingDummy.Name = Guid.NewGuid().ToString();
        await DbContext.RollbackAsync();
        var modifiedDummy = await DbContext.Dummies.FindAsync(existingDummy.Id);

        // Assert 
        modifiedDummy.Should().NotBeNull();
        modifiedDummy.Name.Should().Be(originalName);
    }

    [Test]
    public async Task RollbackAsync_WhenEntityDeleted_ShouldRestoreEntityToOriginalState()
    {
        // Arrange
        var existingDummy = (await PreloadDummiesAsync(1))[0];
        var initialCount = await DbContext.Dummies.CountAsync();

        // Act
        DbContext.Dummies.Remove(existingDummy);
        await DbContext.RollbackAsync();
        var restoredDummy = await DbContext.Dummies.FindAsync(existingDummy.Id);
        var finalCount = await DbContext.Dummies.CountAsync();

        // Assert 
        finalCount.Should().Be(initialCount);
        restoredDummy.Should().NotBeNull();
    }
}

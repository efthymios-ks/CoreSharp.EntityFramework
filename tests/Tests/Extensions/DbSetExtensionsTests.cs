using CoreSharp.EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore;
using Tests.Internal.Database.Models;

namespace Tests.Extensions;

[TestFixture]
public sealed class DbSetExtensionsTests : DummyDbContextTestsBase
{
    [Test]
    public async Task AddManyAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<DummyEntity> dbSet = null;
        var dummiesToAdd = Enumerable.Empty<DummyEntity>();

        // Act
        Func<Task> action = () => dbSet.AddManyAsync(dummiesToAdd);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddManyAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<DummyEntity> dummiesToAdd = null;

        // Act
        Func<Task> action = () => DbContext.Dummies.AddManyAsync(dummiesToAdd);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddManyAsync_WhenCalled_ShouldAddMultipleEntitiesToDatabase()
    {
        // Arrange
        const int toAddCount = 2;
        var dummiesToAdd = GenerateDummies(toAddCount);

        // Act
        var addedDummies = await DbContext.Dummies.AddManyAsync(dummiesToAdd);
        await DbContext.SaveChangesAsync();
        var afterAddCount = await DbContext.Dummies.CountAsync();

        // Assert
        addedDummies.Should().NotBeNull();
        addedDummies.Should().HaveCount(toAddCount);
        afterAddCount.Should().Be(toAddCount);
    }

    [Test]
    public async Task AddManyIfNotExistAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<DummyEntity> dbSet = null;
        var dummiesToAdd = Enumerable.Empty<DummyEntity>();

        // Act
        Func<Task> action = () => dbSet.AddManyIfNotExistAsync(dummiesToAdd);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddManyIfNotExistAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<DummyEntity> dummiesToAdd = null;

        // Act
        Func<Task> action = () => DbContext.Dummies.AddManyIfNotExistAsync(dummiesToAdd);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddManyIfNotExistAsync_WhenEntitiesDoNotExistInDatabase_ShouldAddNewEntities()
    {
        // Arrange
        const int toAddCount = 2;
        var dummiesToAdd = GenerateDummies(toAddCount);

        // Act
        var addedDummies = await DbContext.Dummies.AddManyIfNotExistAsync(dummiesToAdd);
        await DbContext.SaveChangesAsync();
        var afterAddCount = await DbContext.Dummies.CountAsync();

        // Assert
        addedDummies.Should().NotBeNull();
        addedDummies.Should().HaveCount(toAddCount);
        afterAddCount.Should().Be(toAddCount);
    }

    [Test]
    public async Task AddManyIfNotExistAsync_WhenEntitiesAlreadyExistInDatabase_ShouldNotAddEntities()
    {
        // Arrange  
        var existingDummy = (await PreloadDummiesAsync(1))[0];
        var dummyToAdd = GenerateDummy();
        var teachersToTryAdd = new[]
        {
            existingDummy,
            dummyToAdd
        };

        // Act
        var addedDummies = await DbContext.Dummies.AddManyIfNotExistAsync(teachersToTryAdd);
        await DbContext.SaveChangesAsync();
        var afterAddCount = await DbContext.Dummies.CountAsync();

        // Assert
        addedDummies.Should().NotBeNull();
        addedDummies.Should().HaveCount(1);
        addedDummies.Should().ContainEquivalentOf(dummyToAdd);
        afterAddCount.Should().Be(2);
    }

    [Test]
    public async Task UpdateManyAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<DummyEntity> dbSet = null;
        var dummies = Enumerable.Empty<DummyEntity>();

        // Act
        Func<Task> action = () => dbSet.UpdateManyAsync(dummies);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task UpdateManyAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<DummyEntity> dummiesToUpdate = null;

        // Act
        Func<Task> action = () => DbContext.Dummies.UpdateManyAsync(dummiesToUpdate);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task UpdateManyAsync_WhenEntitiesExistInDatabase_ShouldUpdateEntities()
    {
        // Arrange
        const int initialCount = 2;
        var existingDummies = await PreloadDummiesAsync(initialCount);
        foreach (var dummy in existingDummies)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        // Act
        var updatedDummies = await DbContext.Dummies.UpdateManyAsync(existingDummies);
        await DbContext.SaveChangesAsync();

        // Assert
        updatedDummies.Should().NotBeNull();
        updatedDummies.Should().HaveCount(initialCount);
        updatedDummies.Should().BeEquivalentTo(existingDummies);
    }

    [Test]
    public async Task UpdateManyIfExistAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<DummyEntity> dbSet = null;
        var dummiesToUpdate = Enumerable.Empty<DummyEntity>();

        // Act
        Func<Task> action = () => dbSet.UpdateManyIfExistAsync(dummiesToUpdate);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task UpdateManyIfExistAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<DummyEntity> dummiesToUpdate = null;

        // Act
        Func<Task> action = () => DbContext.Dummies.UpdateManyIfExistAsync(dummiesToUpdate);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task UpdateManyIfExistAsync_WhenEntitiesExistInDatabase_ShouldUpdateEntities()
    {
        // Arrange
        const int initialCount = 2;
        var existingDummies = await PreloadDummiesAsync(initialCount);
        foreach (var dummy in existingDummies)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        // Act
        var updatedDummies = await DbContext.Dummies.UpdateManyIfExistAsync(existingDummies);
        await DbContext.SaveChangesAsync();

        // Assert
        updatedDummies.Should().NotBeNull();
        updatedDummies.Should().HaveCount(initialCount);
        updatedDummies.Should().BeEquivalentTo(existingDummies);
    }

    [Test]
    public async Task UpdateManyIfExistAsync_WhenEntitiesDoNotExistInDatabase_ShouldNotUpdateEntities()
    {
        // Arrange  
        var existingDummy = (await PreloadDummiesAsync(1))[0];
        var nonExistingDummy = GenerateDummy();
        var dummiesToUpdate = new[]
        {
            existingDummy,
            nonExistingDummy
        };

        // Act
        existingDummy.Name = Guid.NewGuid().ToString();
        var updatedDummies = await DbContext.Dummies.UpdateManyIfExistAsync(dummiesToUpdate);
        await DbContext.SaveChangesAsync();

        // Assert
        updatedDummies.Should().NotBeNull();
        updatedDummies.Should().HaveCount(1);
        updatedDummies.Should().ContainEquivalentOf(existingDummy);
        updatedDummies.Should().NotContainEquivalentOf(nonExistingDummy);
    }

    [Test]
    public async Task AddOrUpdateManyAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<DummyEntity> dbSet = null;
        var dummiesToAddOrUpdate = Enumerable.Empty<DummyEntity>();

        // Act
        Func<Task> action = () => dbSet.AddOrUpdateManyAsync(dummiesToAddOrUpdate);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddOrUpdateManyAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<DummyEntity> dummiesToAddOrUpdate = null;

        // Act
        Func<Task> action = () => DbContext.Dummies.AddOrUpdateManyAsync(dummiesToAddOrUpdate);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddOrUpdateManyAsync_WhenEntitiesDoNotExistInDatabase_ShouldAddNewEntities()
    {
        // Arrange
        const int toAddCount = 2;
        var dummiesToAdd = GenerateDummies(toAddCount);

        // Act
        var addedDummies = await DbContext.Dummies.AddOrUpdateManyAsync(dummiesToAdd);
        await DbContext.SaveChangesAsync();

        // Assert
        addedDummies.Should().NotBeNull();
        addedDummies.Should().HaveCount(toAddCount);
        addedDummies.Should().BeEquivalentTo(dummiesToAdd);
    }

    [Test]
    public async Task AddOrUpdateManyAsync_WhenEntitiesExistInDatabase_ShouldUpdateExistingEntities()
    {
        // Arrange
        const int initialCount = 2;
        var existingDummies = await PreloadDummiesAsync(initialCount);
        foreach (var dummy in existingDummies)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        // Act
        var updatedDummies = await DbContext.Dummies.AddOrUpdateManyAsync(existingDummies);
        await DbContext.SaveChangesAsync();

        // Assert
        updatedDummies.Should().NotBeNull();
        updatedDummies.Should().HaveCount(initialCount);
        updatedDummies.Should().BeEquivalentTo(existingDummies);
    }

    [Test]
    public async Task AddOrUpdateManyAsync_WhenCalled_ShouldAddNewEntitiesAndUpdateExistingEntities()
    {
        // Arrange
        const int initialCount = 2;
        const int toAddCount = 2;
        var existingDummies = await PreloadDummiesAsync(initialCount);
        foreach (var dummy in existingDummies)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        var dummiesToAdd = GenerateDummies(toAddCount);
        var dummiesToAddOrUpdate = dummiesToAdd.Concat(existingDummies);

        // Act
        var addOrUpdatedDummies = await DbContext.Dummies.AddOrUpdateManyAsync(dummiesToAddOrUpdate);
        await DbContext.SaveChangesAsync();

        // Assert
        addOrUpdatedDummies.Should().NotBeNull();
        addOrUpdatedDummies.Should().HaveCount(initialCount + toAddCount);
        addOrUpdatedDummies.Should().BeEquivalentTo(dummiesToAddOrUpdate);
    }

    [Test]
    public async Task AttachManyAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<DummyEntity> dbSet = null;
        var dummiesToAttach = GenerateDummies(2);

        // Act
        Func<Task> action = () => dbSet.AttachManyAsync(dummiesToAttach);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AttachManyAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        IEnumerable<DummyEntity> dummiesToAttach = null;

        // Act
        Func<Task> action = async () => await DbContext.Dummies.AttachManyAsync(dummiesToAttach);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AttachManyAsync_WhenCalled_ShouldAttachEntitiesToDbContext()
    {
        // Arrange
        const int toAttachCount = 2;
        var dummiesToAttach = GenerateDummies(toAttachCount);

        // Act
        var attachedDummies = await DbContext.Dummies.AttachManyAsync(dummiesToAttach);
        await DbContext.SaveChangesAsync();

        // Assert 
        attachedDummies.Should().NotBeNull();
        attachedDummies.Should().HaveCount(toAttachCount);
        attachedDummies.Should().BeEquivalentTo(attachedDummies);
        foreach (var dummy in attachedDummies)
        {
            var dummyEntityEntry = DbContext.Entry(dummy);
            dummyEntityEntry.State.Should().Be(EntityState.Unchanged);
        }
    }

    [Test]
    public async Task AttachManyIfExistAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<DummyEntity> dbSet = null;
        var dummiesToAttach = GenerateDummies(1);

        // Act
        Func<Task> action = () => dbSet.AttachManyIfExistAsync(dummiesToAttach);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AttachManyIfExistAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<DummyEntity> dummiesToAttach = null;

        // Act
        Func<Task> action = () => DbContext.Dummies.AttachManyIfExistAsync(dummiesToAttach);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AttachManyIfExistAsync_WhenEntitiesExistInDatabase_ShouldAttachExistingEntities()
    {
        // Arrange
        const int initialCount = 2;
        var existingDummies = await PreloadDummiesAsync(initialCount);

        // Act
        var attachedDummies = await DbContext.Dummies.AttachManyIfExistAsync(existingDummies);
        await DbContext.SaveChangesAsync();

        // Assert
        attachedDummies.Should().NotBeNull();
        attachedDummies.Should().HaveCount(initialCount);
        foreach (var dummy in attachedDummies)
        {
            var teacherEntityEntry = DbContext.Entry(dummy);
            teacherEntityEntry.State.Should().Be(EntityState.Unchanged);
        }
    }

    [Test]
    public async Task AttachManyIfExistAsync_WhenEntitiesDoNotExistInDatabase_ShouldNotAttachEntities()
    {
        // Arrange
        var dummiesToAttach = GenerateDummies(2);

        // Act
        var attachedDummies = await DbContext.Dummies.AttachManyIfExistAsync(dummiesToAttach);
        await DbContext.SaveChangesAsync();
        var countAfterAttach = await DbContext.Dummies.CountAsync();

        // Assert
        attachedDummies.Should().NotBeNull();
        attachedDummies.Should().HaveCount(0);
        countAfterAttach.Should().Be(0);
    }

    [Test]
    public async Task AddOrAttachManyAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<DummyEntity> dbSet = null;
        var teachersToAddOrAttach = GenerateDummies(1);

        // Act
        Func<Task> action = () => dbSet.AddOrAttachManyAsync(teachersToAddOrAttach);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddOrAttachManyAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<DummyEntity> teachersToAddOrAttach = null;

        // Act
        Func<Task> action = () => DbContext.Dummies.AddOrAttachManyAsync(teachersToAddOrAttach);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddOrAttachManyAsync_WhenEntitiesDoNotExistInDatabase_ShouldAddNewEntities()
    {
        // Arrange
        const int toAddCount = 2;
        var dummiesToAdd = GenerateDummies(toAddCount);

        // Act
        var addedDummies = await DbContext.Dummies.AddOrAttachManyAsync(dummiesToAdd);
        await DbContext.SaveChangesAsync();

        // Assert
        addedDummies.Should().NotBeNull();
        addedDummies.Should().HaveCount(toAddCount);
        addedDummies.Should().BeEquivalentTo(dummiesToAdd);
    }

    [Test]
    public async Task AddOrAttachManyAsync_WhenEntitiesExistInDatabase_ShouldAttachExistingEntities()
    {
        // Arrange
        const int initialCount = 2;
        var existingDummies = await PreloadDummiesAsync(initialCount);

        // Act
        var attachedDummies = await DbContext.Dummies.AddOrAttachManyAsync(existingDummies);
        await DbContext.SaveChangesAsync();

        // Assert
        attachedDummies.Should().NotBeNull();
        attachedDummies.Should().HaveCount(initialCount);
        foreach (var dummy in attachedDummies)
        {
            var teacherEntityEntry = DbContext.Entry(dummy);
            teacherEntityEntry.State.Should().Be(EntityState.Unchanged);
        }
    }

    [Test]
    public async Task AddOrAttachManyAsync_WhenCalled_ShouldAddNewEntitiesAndUpdateExistingEntities()
    {
        // Arrange
        const int initialCount = 5;
        const int toAddCount = 2;
        var existingDummies = await PreloadDummiesAsync(initialCount);
        foreach (var dummy in existingDummies)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        var dummiesToAdd = GenerateDummies(toAddCount);
        var teachersToAddOrAttach = dummiesToAdd.Concat(existingDummies);

        // Act
        var addedOrAttachedDummies = await DbContext.Dummies.AddOrAttachManyAsync(teachersToAddOrAttach);
        await DbContext.SaveChangesAsync();

        // Assert
        addedOrAttachedDummies.Should().NotBeNull();
        addedOrAttachedDummies.Should().HaveCount(initialCount + toAddCount);
        DbContext.Dummies.Should().BeEquivalentTo(teachersToAddOrAttach);
    }

    [Test]
    public async Task RemoveByKeyAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<DummyEntity> dbSet = null;
        var dummiesToRemove = GenerateDummy();

        // Act
        Func<Task> action = () => dbSet.RemoveByKeyAsync(dummiesToRemove.Id);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task RemoveByKeyAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        object dummyIdToRemove = null;

        // Act
        Func<Task> action = () => DbContext.Dummies.RemoveByKeyAsync(dummyIdToRemove);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task RemoveByKeyAsync_WhenKeyExists_ShouldRemoveEntityFromDbContext()
    {
        // Arrange 
        var existingDummy = (await PreloadDummiesAsync(1))[0];

        // Act
        var dummiesRemovedCount = await DbContext.Dummies.RemoveByKeyAsync(existingDummy.Id);
        await DbContext.SaveChangesAsync();

        // Assert
        dummiesRemovedCount.Should().Be(1);
    }

    [Test]
    public async Task RemoveByKeyAsync_WhenKeyNotFound_ShouldNotRemoveEntityFromDbContext()
    {
        // Arrange 
        var dummiesBeforeRemove = await PreloadDummiesAsync(1);

        // Act
        var dummiesRemovedCount = await DbContext.Dummies.RemoveByKeyAsync(Guid.NewGuid());
        await DbContext.SaveChangesAsync();
        var dummiesAfterRemove = await DbContext.Dummies.ToArrayAsync();

        // Assert
        dummiesRemovedCount.Should().Be(0);
        dummiesAfterRemove.Should().BeEquivalentTo(dummiesBeforeRemove);
    }

    [Test]
    public async Task RemoveManyAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<DummyEntity> dbSet = null;
        var dummiesToRemove = GenerateDummies(1);

        // Act
        Func<Task> action = () => dbSet.RemoveManyAsync(dummiesToRemove);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task RemoveManyAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<DummyEntity> dummiesToRemove = null;

        // Act
        Func<Task> action = () => DbContext.Dummies.RemoveManyAsync(dummiesToRemove);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task RemoveManyAsync_WhenCalled_ShouldRemoveEntitiesFromDbContext()
    {
        // Arrange
        const int initialCount = 5;
        const int toRemoveCount = 3;
        var existingDummies = await PreloadDummiesAsync(initialCount);
        var dummiesToRemove = existingDummies.Take(toRemoveCount);

        // Act
        await DbContext.Dummies.RemoveManyAsync(dummiesToRemove);
        await DbContext.SaveChangesAsync();
        var dummiesAfterRemove = await DbContext.Dummies.ToArrayAsync();

        // Assert
        dummiesAfterRemove.Should().HaveCount(initialCount - toRemoveCount);
        dummiesAfterRemove.Should().NotContain(dummiesToRemove);
    }
}


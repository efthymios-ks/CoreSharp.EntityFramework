using CoreSharp.EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore;
using Tests.Internal.Database.Models;

namespace Tests.Extensions;

[TestFixture]
public sealed class DbSetExtensionsTests : DummyDbContextTestsBase
{
    // AddManyAsync
    [Test]
    public async Task AddManyAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<DummyEntity> dbSet = null;
        var dummiesToAdd = Enumerable.Empty<DummyEntity>();

        // Act
        Func<Task> action = () => dbSet.AddManyAsync<DummyEntity, Guid>(dummiesToAdd);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddManyAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<DummyEntity> dummiesToAdd = null;

        // Act
        Func<Task> action = () => DbContext.Dummies.AddManyAsync<DummyEntity, Guid>(dummiesToAdd);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddManyAsync_WhenEntitiesDoNotExistInDatabase_ShouldSetEntitiesAsAddedAndReturnThem()
    {
        // Arrange 
        var dummiesToAdd = GenerateDummies(1);

        // Act
        var dummiesReturned = await DbContext.Dummies.AddManyAsync<DummyEntity, Guid>(dummiesToAdd);
        var dummyEntries = GetDummyEntries(EntityState.Added);

        // Assert
        dummiesReturned.Should().BeEquivalentTo(dummiesToAdd);
        dummyEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToAdd);
    }

    [Test]
    public async Task AddManyAsync_WhenEntitiesExistInDatabase_ShouldSetEntitiesAsAddedAndReturnThem()
    {
        // Arrange 
        var dummiesToAdd = await PreloadDummiesAsync(1);

        // Act
        var dummiesReturned = await DbContext.Dummies.AddManyAsync<DummyEntity, Guid>(dummiesToAdd);
        var dummyEntries = GetDummyEntries(EntityState.Added);

        // Assert
        dummiesReturned.Should().BeEquivalentTo(dummiesToAdd);
        dummyEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToAdd);
    }

    // AttachManyAsync
    [Test]
    public async Task AttachManyAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<DummyEntity> dbSet = null;
        var dummiesToAttach = GenerateDummies(1);

        // Act
        Func<Task> action = () => dbSet.AttachManyAsync<DummyEntity, Guid>(dummiesToAttach);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AttachManyAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        IEnumerable<DummyEntity> dummiesToAttach = null;

        // Act
        Func<Task> action = () => DbContext.Dummies.AttachManyAsync<DummyEntity, Guid>(dummiesToAttach);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AttachManyAsync_WhenEntitiesExistInDatabase_ShouldSetEntitiesAsModifiedAndReturnThem()
    {
        // Arrange 
        var dummiesToAttach = await PreloadDummiesAsync(1);
        foreach (var dummy in dummiesToAttach)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        // Act
        var dummiesReturned = await DbContext.Dummies.AttachManyAsync<DummyEntity, Guid>(dummiesToAttach);
        var dummyEntries = GetDummyEntries(EntityState.Modified);

        // Assert
        dummiesReturned.Should().BeEquivalentTo(dummiesToAttach);
        dummyEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToAttach);
    }

    [Test(Description = "By default, 'DbContext.Attach' will add and start tracking entities when PK is not found.")]
    public async Task AttachManyAsync_WhenEntitiesDoNotExistInDatabase_ShouldSetEntitiesAsAddedAndReturnThem()
    {
        // Arrange 
        var dummiesToAttach = GenerateDummies(1);
        foreach (var dummy in dummiesToAttach)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        // Act
        var dummiesReturned = await DbContext.Dummies.AttachManyAsync<DummyEntity, Guid>(dummiesToAttach);
        var dummyEntries = GetDummyEntries(EntityState.Added);

        // Assert
        dummiesReturned.Should().BeEquivalentTo(dummiesToAttach);
        dummyEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToAttach);
    }

    // UpdateManyAsync
    [Test]
    public async Task UpdateManyAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<DummyEntity> dbSet = null;
        var dummies = Enumerable.Empty<DummyEntity>();

        // Act
        Func<Task> action = () => dbSet.UpdateManyAsync<DummyEntity, Guid>(dummies);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task UpdateManyAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<DummyEntity> dummiesToUpdate = null;

        // Act
        Func<Task> action = () => DbContext.Dummies.UpdateManyAsync<DummyEntity, Guid>(dummiesToUpdate);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task UpdateManyAsync_WhenEntitiesExistInDatabase_ShouldSetEntitiesAsModifiedAndReturnThem()
    {
        // Arrange 
        var dummiesToUpdate = await PreloadDummiesAsync(1);
        foreach (var dummy in dummiesToUpdate)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        // Act
        var dummiesReturned = await DbContext.Dummies.UpdateManyAsync<DummyEntity, Guid>(dummiesToUpdate);
        var dummyEntries = GetDummyEntries(EntityState.Modified);

        // Assert
        dummiesReturned.Should().BeEquivalentTo(dummiesToUpdate);
        dummyEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToUpdate);
    }

    [Test(Description = "By default, 'DbContext.Update' will add and start tracking entities when PK is not found.")]
    public async Task UpdateManyAsync_WhenEntitiesDoNotExistInDatabase_ShouldSetEntitiesAsAddedAndReturnThem()
    {
        // Arrange 
        var dummiesToUpdate = GenerateDummies(1);
        foreach (var dummy in dummiesToUpdate)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        // Act
        var dummiesReturned = await DbContext.Dummies.UpdateManyAsync<DummyEntity, Guid>(dummiesToUpdate);
        var dummyEntries = GetDummyEntries(EntityState.Added);

        // Assert
        dummiesReturned.Should().BeEquivalentTo(dummiesToUpdate);
        dummyEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToUpdate);
    }

    // RemoveManyAsync
    [Test]
    public async Task RemoveManyAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<DummyEntity> dbSet = null;
        var dummiesToUpdate = Enumerable.Empty<DummyEntity>();

        // Act
        Func<Task> action = () => dbSet.RemoveManyAsync<DummyEntity, Guid>(dummiesToUpdate);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task RemoveManyAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<DummyEntity> dummiesToRemove = null;

        // Act
        Func<Task> action = () => DbContext.Dummies.RemoveManyAsync<DummyEntity, Guid>(dummiesToRemove);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task RemoveManyAsync_WhenEntitiesExistInDatabase_ShouldSetEntitiesAsDeleted()
    {
        // Arrange 
        var dummiesToRemove = await PreloadDummiesAsync(1);

        // Act
        await DbContext.Dummies.RemoveManyAsync<DummyEntity, Guid>(dummiesToRemove);
        var dummyEntries = GetDummyEntries(EntityState.Deleted);

        // Assert  
        dummyEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToRemove);
    }

    [Test]
    public async Task RemoveManyAsync_WhenEntitiesDoNotExistInDatabase_ShouldDoNothing()
    {
        // Arrange 
        var dummiesToRemove = GenerateDummies(1);

        // Act
        await DbContext.Dummies.RemoveManyAsync<DummyEntity, Guid>(dummiesToRemove);
        var dummyEntries = GetDummyEntries(EntityState.Deleted);

        // Assert  
        dummyEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToRemove);
    }

    // RemoveByKeyAsync
    [Test]
    public async Task RemoveAsync_ByKey_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<DummyEntity> dbSet = null;
        var dummyToRemove = await PreloadDummyAsync();

        // Act
        Func<Task> action = () => dbSet.RemoveByKeyAsync(dummyToRemove.Id);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task RemoveAsync_ByKey_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange
        var existingDummy = await PreloadDummyAsync();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => DbContext.Dummies.RemoveByKeyAsync(existingDummy.Id, cancellationTokenSource.Token);

        // Assert 
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task RemoveAsync_ByKey_WhenKeyExists_ShouldRemoveEntityFromDbContext()
    {
        // Arrange 
        var existingDummy = await PreloadDummyAsync();

        // Act
        var removed = await DbContext.Dummies.RemoveByKeyAsync(existingDummy.Id);
        var dummyEntry = GetDummyEntry(EntityState.Deleted);

        // Assert 
        removed.Should().BeTrue();
        dummyEntry.Should().NotBeNull();
        dummyEntry.Entity.Should().BeEquivalentTo(existingDummy);
    }

    [Test]
    public async Task RemoveAsync_ByKey_WhenKeyNotFound_ShouldNotRemoveEntityFromDbContext()
    {
        // Arrange
        var dummyIdToRemove = Guid.NewGuid();

        // Act
        var removed = await DbContext.Dummies.RemoveByKeyAsync(dummyIdToRemove);
        var dummyEntry = GetDummyEntry(EntityState.Deleted);

        // Assert 
        removed.Should().BeFalse();
        dummyEntry.Should().BeNull();
    }

    // AddManyIfExistAsync
    [Test]
    public async Task AddManyIfNotExistAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<DummyEntity> dbSet = null;
        var dummiesToAdd = Enumerable.Empty<DummyEntity>();

        // Act
        Func<Task> action = () => dbSet.AddManyIfNotExistAsync<DummyEntity, Guid>(dummiesToAdd);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddManyIfNotExistAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<DummyEntity> dummiesToAdd = null;

        // Act
        Func<Task> action = () => DbContext.Dummies.AddManyIfNotExistAsync<DummyEntity, Guid>(dummiesToAdd);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddManyIfNotExistAsync_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var dummiesToAdd = GenerateDummies(1);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => DbContext.Dummies.AddManyIfNotExistAsync<DummyEntity, Guid>(dummiesToAdd, cancellationTokenSource.Token);

        // Assert
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task AddManyIfNotExistAsync_WhenEntitiesDoNotExistInDatabase_ShouldSetEntitiesAsAddedAndReturnThem()
    {
        // Arrange 
        var dummiesToAdd = GenerateDummies(1);

        // Act
        var dummiesReturned = await DbContext.Dummies.AddManyIfNotExistAsync<DummyEntity, Guid>(dummiesToAdd);
        var dummyEntries = GetDummyEntries(EntityState.Added);

        // Assert
        dummiesReturned.Should().BeEquivalentTo(dummiesToAdd);
        dummyEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToAdd);
    }

    [Test]
    public async Task AddManyIfNotExistAsync_WhenEntitiesExistInDatabase_ShouldDoNothing()
    {
        // Arrange  
        var dummiesToAdd = await PreloadDummiesAsync(1);

        // Act
        var dummiesReturned = await DbContext.Dummies.AddManyIfNotExistAsync<DummyEntity, Guid>(dummiesToAdd);
        var dummyEntries = GetDummyEntries(EntityState.Added);

        // Assert 
        dummiesReturned.Should().BeEmpty();
        dummyEntries.Should().BeEmpty();
    }

    // AttachManyIfExistAsync
    [Test]
    public async Task AttachManyIfExistAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<DummyEntity> dbSet = null;
        var dummiesToUpdate = Enumerable.Empty<DummyEntity>();

        // Act
        Func<Task> action = () => dbSet.AttachManyIfExistAsync<DummyEntity, Guid>(dummiesToUpdate);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AttachManyIfExistAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<DummyEntity> dummiesToAttach = null;

        // Act
        Func<Task> action = () => DbContext.Dummies.AttachManyIfExistAsync<DummyEntity, Guid>(dummiesToAttach);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AttachManyIfExistAsync_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var dummiesToAttach = await PreloadDummiesAsync(1);
        foreach (var dummy in dummiesToAttach)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => DbContext.Dummies.AttachManyIfExistAsync<DummyEntity, Guid>(dummiesToAttach, cancellationTokenSource.Token);

        // Assert
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task AttachManyIfExistAsync_WhenEntitiesExistInDatabase_ShouldSetEntitiesAsModifiedAndReturnThem()
    {
        // Arrange 
        var dummiesToAttach = await PreloadDummiesAsync(1);
        foreach (var dummy in dummiesToAttach)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        // Act
        var dummiesReturned = await DbContext.Dummies.AttachManyIfExistAsync<DummyEntity, Guid>(dummiesToAttach);
        var dummyEntries = GetDummyEntries(EntityState.Modified);

        // Assert 
        dummiesReturned.Should().BeEquivalentTo(dummiesToAttach);
        dummyEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToAttach);
    }

    [Test]
    public async Task AttachManyIfExistAsync_WhenEntitiesDoNotExistInDatabase_ShouldDoNothing()
    {
        // Arrange  
        var dummiesToAttach = GenerateDummies(1);

        // Act
        var dummiesReturned = await DbContext.Dummies.AttachManyIfExistAsync<DummyEntity, Guid>(dummiesToAttach);
        var dummyEntries = GetDummyEntries(EntityState.Modified);

        // Assert
        dummiesReturned.Should().BeEmpty();
        dummyEntries.Should().BeEmpty();
    }

    // UpdateManyIfExist
    [Test]
    public async Task UpdateManyIfExistAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<DummyEntity> dbSet = null;
        var dummiesToUpdate = Enumerable.Empty<DummyEntity>();

        // Act
        Func<Task> action = () => dbSet.UpdateManyIfExistAsync<DummyEntity, Guid>(dummiesToUpdate);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task UpdateManyIfExistAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<DummyEntity> dummiesToUpdate = null;

        // Act
        Func<Task> action = () => DbContext.Dummies.UpdateManyIfExistAsync<DummyEntity, Guid>(dummiesToUpdate);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task UpdateManyIfExistAsync_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var dummiesToUpdate = await PreloadDummiesAsync(1);
        foreach (var dummy in dummiesToUpdate)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => DbContext.Dummies.UpdateManyIfExistAsync<DummyEntity, Guid>(dummiesToUpdate, cancellationTokenSource.Token);

        // Assert
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task UpdateManyIfExistAsync_WhenEntitiesExistInDatabase_ShouldSetEntitiesAsModifiedAndReturnThem()
    {
        // Arrange 
        var dummiesToUpdate = await PreloadDummiesAsync(1);
        foreach (var dummy in dummiesToUpdate)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        // Act
        var dummiesReturned = await DbContext.Dummies.UpdateManyIfExistAsync<DummyEntity, Guid>(dummiesToUpdate);
        var dummyEntries = GetDummyEntries(EntityState.Modified);

        // Assert 
        dummiesReturned.Should().BeEquivalentTo(dummiesToUpdate);
        dummyEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToUpdate);
    }

    [Test]
    public async Task UpdateManyIfExistAsync_WhenEntitiesDoNotExistInDatabase_ShouldDoNothing()
    {
        // Arrange  
        var dummiesToUpdate = GenerateDummies(1);

        // Act
        var dummiesReturned = await DbContext.Dummies.UpdateManyIfExistAsync<DummyEntity, Guid>(dummiesToUpdate);
        var dummyEntries = GetDummyEntries(EntityState.Modified);

        // Assert
        dummiesReturned.Should().BeEmpty();
        dummyEntries.Should().BeEmpty();
    }

    // AddOrAttachManyAsync
    [Test]
    public async Task AddOrAttachManyAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<DummyEntity> dbSet = null;
        var dummiesToAddOrUpdate = Enumerable.Empty<DummyEntity>();

        // Act
        Func<Task> action = () => dbSet.AddOrAttachManyAsync<DummyEntity, Guid>(dummiesToAddOrUpdate);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddOrAttachManyAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<DummyEntity> dummiesToAddOrAttach = null;

        // Act
        Func<Task> action = () => DbContext.Dummies.AddOrAttachManyAsync<DummyEntity, Guid>(dummiesToAddOrAttach);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddOrAttachManyAsync_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var dummiesToAddOrAttach = GenerateDummies(1);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => DbContext.Dummies.AddOrAttachManyAsync<DummyEntity, Guid>(dummiesToAddOrAttach, cancellationTokenSource.Token);

        // Assert 
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task AddOrAttachManyAsync_WhenEntitiesDoNotExistInDatabase_ShouldSetEntitiesAsAddedAndReturnThem()
    {
        // Arrange 
        var dummiesToAdd = GenerateDummies(1);

        // Act
        var dummiesReturned = await DbContext.Dummies.AddOrAttachManyAsync<DummyEntity, Guid>(dummiesToAdd);
        var dummyEntries = GetDummyEntries(EntityState.Added);

        // Assert 
        dummiesReturned.Should().BeEquivalentTo(dummiesToAdd);
        dummyEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToAdd);
    }

    [Test]
    public async Task AddOrAttachManyAsync_WhenEntitiesExistInDatabase_ShouldSetEntitiesAsModifiedAndReturnThem()
    {
        // Arrange 
        var dummiesToAttach = await PreloadDummiesAsync(1);
        foreach (var dummy in dummiesToAttach)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        // Act
        var dummiesReturned = await DbContext.Dummies.AddOrAttachManyAsync<DummyEntity, Guid>(dummiesToAttach);
        var dummyEntries = GetDummyEntries(EntityState.Modified);

        // Assert 
        dummiesReturned.Should().BeEquivalentTo(dummiesToAttach);
        dummyEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToAttach);
    }

    [Test]
    public async Task AddOrAttachManyAsync_WhenMixedEntities_ShouldSetNewEntitiesAsAddedAndExistingEntitiesAsModifiedAndReturnThem()
    {
        // Arrange 
        var dummiesToAttach = await PreloadDummiesAsync(1);
        foreach (var dummy in dummiesToAttach)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        var dummiesToAdd = GenerateDummies(1);
        var dummiesToAddOrAttach = dummiesToAttach.Concat(dummiesToAdd).ToArray();

        // Act
        var dummiesReturned = await DbContext.Dummies.AddOrAttachManyAsync<DummyEntity, Guid>(dummiesToAddOrAttach);
        var addedEntries = GetDummyEntries(EntityState.Added);
        var mofidiedEntries = GetDummyEntries(EntityState.Modified);

        // Assert 
        dummiesReturned.Should().BeEquivalentTo(dummiesToAddOrAttach);
        addedEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToAdd);
        mofidiedEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToAttach);
    }

    // AddOrUpdateManyAsync
    [Test]
    public async Task AddOrUpdateManyAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<DummyEntity> dbSet = null;
        var dummiesToAddOrUpdate = Enumerable.Empty<DummyEntity>();

        // Act
        Func<Task> action = () => dbSet.AddOrUpdateManyAsync<DummyEntity, Guid>(dummiesToAddOrUpdate);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddOrUpdateManyAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<DummyEntity> dummiesToAddOrUpdate = null;

        // Act
        Func<Task> action = () => DbContext.Dummies.AddOrUpdateManyAsync<DummyEntity, Guid>(dummiesToAddOrUpdate);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddOrUpdateManyAsync_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var dummiesToAddOrUpdate = GenerateDummies(1);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> action = () => DbContext.Dummies.AddOrUpdateManyAsync<DummyEntity, Guid>(dummiesToAddOrUpdate, cancellationTokenSource.Token);

        // Assert 
        await action.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task AddOrUpdateManyAsync_WhenEntitiesDoNotExistInDatabase_ShouldSetEntitiesAsAddedAndReturnThem()
    {
        // Arrange 
        var dummiesToAdd = GenerateDummies(1);

        // Act
        var dummiesReturned = await DbContext.Dummies.AddOrUpdateManyAsync<DummyEntity, Guid>(dummiesToAdd);
        var dummyEntries = GetDummyEntries(EntityState.Added);

        // Assert 
        dummiesReturned.Should().BeEquivalentTo(dummiesToAdd);
        dummyEntries.Should().AllSatisfy(entry => entry.State.Should().Be(EntityState.Added));
        dummyEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToAdd);
    }

    [Test]
    public async Task AddOrUpdateManyAsync_WhenEntitiesExistInDatabase_ShouldSetEntitiesAsModifiedAndReturnThem()
    {
        // Arrange 
        var dummiesToUpdate = await PreloadDummiesAsync(1);
        foreach (var dummy in dummiesToUpdate)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        // Act
        var dummiesReturned = await DbContext.Dummies.AddOrUpdateManyAsync<DummyEntity, Guid>(dummiesToUpdate);
        var dummyEntries = GetDummyEntries(EntityState.Modified);

        // Assert 
        dummiesReturned.Should().BeEquivalentTo(dummiesToUpdate);
        dummyEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToUpdate);
    }

    [Test]
    public async Task AddOrUpdateManyAsync_WhenMixedEntities_ShouldSetNewEntitiesAsAddedAndExistingEntitiesAsModifiedAndReturnThem()
    {
        // Arrange 
        var dummiesToUpdate = await PreloadDummiesAsync(1);
        foreach (var dummy in dummiesToUpdate)
        {
            dummy.Name = Guid.NewGuid().ToString();
        }

        var dummiesToAdd = GenerateDummies(1);
        var dummiesToAddOrUpdate = dummiesToUpdate.Concat(dummiesToAdd).ToArray();

        // Act
        var dummiesReturned = await DbContext.Dummies.AddOrUpdateManyAsync<DummyEntity, Guid>(dummiesToAddOrUpdate);
        var addedEntries = GetDummyEntries(EntityState.Added);
        var mofidiedEntries = GetDummyEntries(EntityState.Modified);

        // Assert 
        dummiesReturned.Should().BeEquivalentTo(dummiesToAddOrUpdate);
        addedEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToAdd);
        mofidiedEntries.Select(entry => entry.Entity).Should().BeEquivalentTo(dummiesToUpdate);
    }
}


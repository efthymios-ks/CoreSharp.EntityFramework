using CoreSharp.EntityFramework.Extensions;
using CoreSharp.EntityFramework.Tests.Internal.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Tests.Extensions;


[Collection(nameof(SharedSqlServerCollection))]
public sealed class DbSetExtensionsTests(SharedSqlServerContainer sqlContainer)
    : SharedSqlServerTestsBase(sqlContainer)
{
    // AddManyAsync
    [Fact]
    public async Task AddManyAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<DummyEntity> dbSet = null!;
        var dummiesToAdd = Enumerable.Empty<DummyEntity>();

        // Act
        Task Action()
            => dbSet.AddManyAsync<DummyEntity, Guid>(dummiesToAdd);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task AddManyAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<DummyEntity> dummiesToAdd = null!;

        // Act
        Task Action()
            => DbContext.Dummies.AddManyAsync<DummyEntity, Guid>(dummiesToAdd);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task AddManyAsync_WhenEntitiesDoNotExistInDatabase_ShouldSetEntitiesAsAddedAndReturnThem()
    {
        // Arrange 
        var dummiesToAdd = GenerateDummies(1);

        // Act
        var dummiesReturned = await DbContext.Dummies.AddManyAsync<DummyEntity, Guid>(dummiesToAdd);
        var dummyEntries = GetDummyEntries(EntityState.Added);

        // Assert
        Assert.Equivalent(dummiesToAdd, dummiesReturned);
        Assert.Equivalent(dummiesToAdd, dummyEntries.Select(entry => entry.Entity));
    }

    [Fact]
    public async Task AddManyAsync_WhenEntitiesExistInDatabase_ShouldSetEntitiesAsAddedAndReturnThem()
    {
        // Arrange 
        var dummiesToAdd = await PreloadDummiesAsync(1);

        // Act
        var dummiesReturned = await DbContext.Dummies.AddManyAsync<DummyEntity, Guid>(dummiesToAdd);
        var dummyEntries = GetDummyEntries(EntityState.Added);

        // Assert
        Assert.Equivalent(dummiesToAdd, dummiesReturned);
        Assert.Equivalent(dummiesToAdd, dummyEntries.Select(entry => entry.Entity));
    }

    // AttachManyAsync
    [Fact]
    public async Task AttachManyAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<DummyEntity> dbSet = null!;
        var dummiesToAttach = GenerateDummies(1);

        // Act
        Task Action()
            => dbSet.AttachManyAsync<DummyEntity, Guid>(dummiesToAttach);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task AttachManyAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        IEnumerable<DummyEntity> dummiesToAttach = null!;

        // Act
        Task Action()
            => DbContext.Dummies.AttachManyAsync<DummyEntity, Guid>(dummiesToAttach);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
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
        Assert.Equivalent(dummiesToAttach, dummiesReturned);
        Assert.Equivalent(dummiesToAttach, dummyEntries.Select(entry => entry.Entity));
    }

    /// <summary>
    /// By default, 'DbContext.Attach' will add and start tracking entities when PK is not found.
    /// </summary> 
    [Fact]
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
        Assert.Equivalent(dummiesToAttach, dummiesReturned);
        Assert.Equivalent(dummiesToAttach, dummyEntries.Select(entry => entry.Entity));
    }

    // UpdateManyAsync
    [Fact]
    public async Task UpdateManyAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<DummyEntity> dbSet = null!;
        var dummies = Enumerable.Empty<DummyEntity>();

        // Act
        Task Action()
            => dbSet!.UpdateManyAsync<DummyEntity, Guid>(dummies);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task UpdateManyAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<DummyEntity> dummiesToUpdate = null!;

        // Act
        Task Action()
            => DbContext.Dummies.UpdateManyAsync<DummyEntity, Guid>(dummiesToUpdate);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
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
        Assert.Equivalent(dummiesToUpdate, dummiesReturned);
        Assert.Equivalent(dummiesToUpdate, dummyEntries.Select(entry => entry.Entity));
    }

    /// <summary>
    /// By default, 'DbContext.Update' will add and start tracking entities when PK is not found.
    /// </summary>
    [Fact]
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
        Assert.Equivalent(dummiesToUpdate, dummiesReturned);
        Assert.Equivalent(dummiesToUpdate, dummyEntries.Select(entry => entry.Entity));
    }

    // RemoveManyAsync
    [Fact]
    public async Task RemoveManyAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<DummyEntity> dbSet = null!;
        var dummiesToUpdate = Enumerable.Empty<DummyEntity>();

        // Act
        Task Action()
            => dbSet.RemoveManyAsync<DummyEntity, Guid>(dummiesToUpdate);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task RemoveManyAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<DummyEntity> dummiesToRemove = null!;

        // Act
        Task Action()
            => DbContext.Dummies.RemoveManyAsync<DummyEntity, Guid>(dummiesToRemove);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task RemoveManyAsync_WhenEntitiesExistInDatabase_ShouldSetEntitiesAsDeleted()
    {
        // Arrange 
        var dummiesToRemove = await PreloadDummiesAsync(1);

        // Act
        await DbContext.Dummies.RemoveManyAsync<DummyEntity, Guid>(dummiesToRemove);
        var dummyEntries = GetDummyEntries(EntityState.Deleted);

        // Assert  
        Assert.Equivalent(dummiesToRemove, dummyEntries.Select(entry => entry.Entity));
    }

    [Fact]
    public async Task RemoveManyAsync_WhenEntitiesDoNotExistInDatabase_ShouldDoNothing()
    {
        // Arrange 
        var dummiesToRemove = GenerateDummies(1);

        // Act
        await DbContext.Dummies.RemoveManyAsync<DummyEntity, Guid>(dummiesToRemove);
        var dummyEntries = GetDummyEntries(EntityState.Deleted);

        // Assert  
        Assert.Equivalent(dummiesToRemove, dummyEntries.Select(entry => entry.Entity));
    }

    // RemoveByKeyAsync
    [Fact]
    public async Task RemoveAsync_ByKey_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<DummyEntity> dbSet = null!;
        var dummyToRemove = await PreloadDummyAsync();

        // Act
        Task Action()
            => dbSet.RemoveByKeyAsync(dummyToRemove.Id);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task RemoveAsync_ByKey_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange
        var existingDummy = await PreloadDummyAsync();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Task Action()
            => DbContext.Dummies.RemoveByKeyAsync(existingDummy.Id, cancellationTokenSource.Token);

        // Assert 
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task RemoveAsync_ByKey_WhenKeyExists_ShouldRemoveEntityFromDbContext()
    {
        // Arrange 
        var existingDummy = await PreloadDummyAsync();

        // Act
        var removed = await DbContext.Dummies.RemoveByKeyAsync(existingDummy.Id);
        var dummyEntry = GetDummyEntry(EntityState.Deleted)!;

        // Assert 
        Assert.True(removed);
        Assert.NotNull(dummyEntry);
        Assert.Equivalent(existingDummy, dummyEntry.Entity);
    }

    [Fact]
    public async Task RemoveAsync_ByKey_WhenKeyNotFound_ShouldNotRemoveEntityFromDbContext()
    {
        // Arrange
        var dummyIdToRemove = Guid.NewGuid();

        // Act
        var removed = await DbContext.Dummies.RemoveByKeyAsync(dummyIdToRemove);
        var dummyEntry = GetDummyEntry(EntityState.Deleted);

        // Assert 
        Assert.False(removed);
        Assert.Null(dummyEntry);
    }

    // AddManyIfExistAsync
    [Fact]
    public async Task AddManyIfNotExistAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<DummyEntity> dbSet = null!;
        var dummiesToAdd = Enumerable.Empty<DummyEntity>();

        // Act
        async Task Action()
            => await dbSet.AddManyIfNotExistAsync<DummyEntity, Guid>(dummiesToAdd);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task AddManyIfNotExistAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<DummyEntity> dummiesToAdd = null!;

        // Act
        Task Action()
            => DbContext.Dummies.AddManyIfNotExistAsync<DummyEntity, Guid>(dummiesToAdd);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task AddManyIfNotExistAsync_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var dummiesToAdd = GenerateDummies(1);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        async Task Action()
            => await DbContext.Dummies.AddManyIfNotExistAsync<DummyEntity, Guid>(dummiesToAdd, cancellationTokenSource.Token);

        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task AddManyIfNotExistAsync_WhenEntitiesDoNotExistInDatabase_ShouldSetEntitiesAsAddedAndReturnThem()
    {
        // Arrange 
        var dummiesToAdd = GenerateDummies(1);

        // Act
        var dummiesReturned = await DbContext.Dummies.AddManyIfNotExistAsync<DummyEntity, Guid>(dummiesToAdd);
        var dummyEntries = GetDummyEntries(EntityState.Added);

        // Assert
        Assert.Equivalent(dummiesToAdd, dummiesReturned);
        Assert.Equivalent(dummiesToAdd, dummyEntries.Select(entry => entry.Entity));
    }

    [Fact]
    public async Task AddManyIfNotExistAsync_WhenEntitiesExistInDatabase_ShouldDoNothing()
    {
        // Arrange  
        var dummiesToAdd = await PreloadDummiesAsync(1);

        // Act
        var dummiesReturned = await DbContext.Dummies.AddManyIfNotExistAsync<DummyEntity, Guid>(dummiesToAdd);
        var dummyEntries = GetDummyEntries(EntityState.Added);

        // Assert 
        Assert.Empty(dummiesReturned);
        Assert.Empty(dummyEntries);
    }

    // AttachManyIfExistAsync
    [Fact]
    public async Task AttachManyIfExistAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<DummyEntity> dbSet = null!;
        var dummiesToUpdate = Enumerable.Empty<DummyEntity>();

        // Act
        async Task Action()
            => await dbSet.AttachManyIfExistAsync<DummyEntity, Guid>(dummiesToUpdate);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task AttachManyIfExistAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<DummyEntity> dummiesToAttach = null!;

        // Act
        async Task Action()
            => await DbContext.Dummies.AttachManyIfExistAsync<DummyEntity, Guid>(dummiesToAttach);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
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
        async Task Action()
            => await DbContext.Dummies.AttachManyIfExistAsync<DummyEntity, Guid>(dummiesToAttach, cancellationTokenSource.Token);

        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
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
        Assert.Equivalent(dummiesToAttach, dummiesReturned);
        Assert.Equivalent(dummiesToAttach, dummyEntries.Select(entry => entry.Entity));
    }

    [Fact]
    public async Task AttachManyIfExistAsync_WhenEntitiesDoNotExistInDatabase_ShouldDoNothing()
    {
        // Arrange  
        var dummiesToAttach = GenerateDummies(1);

        // Act
        var dummiesReturned = await DbContext.Dummies.AttachManyIfExistAsync<DummyEntity, Guid>(dummiesToAttach);
        var dummyEntries = GetDummyEntries(EntityState.Modified);

        // Assert
        Assert.Empty(dummiesReturned);
        Assert.Empty(dummyEntries);
    }

    // UpdateManyIfExist
    [Fact]
    public async Task UpdateManyIfExistAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<DummyEntity> dbSet = null!;
        var dummiesToUpdate = Enumerable.Empty<DummyEntity>();

        // Act
        async Task Action()
            => await dbSet.UpdateManyIfExistAsync<DummyEntity, Guid>(dummiesToUpdate);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task UpdateManyIfExistAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<DummyEntity> dummiesToUpdate = null!;

        // Act
        async Task Action()
            => await DbContext.Dummies.UpdateManyIfExistAsync<DummyEntity, Guid>(dummiesToUpdate);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
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
        async Task Action()
            => await DbContext.Dummies.UpdateManyIfExistAsync<DummyEntity, Guid>(dummiesToUpdate, cancellationTokenSource.Token);

        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
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
        Assert.Equivalent(dummiesToUpdate, dummiesReturned);
        Assert.Equivalent(dummiesToUpdate, dummyEntries.Select(entry => entry.Entity));
    }

    [Fact]
    public async Task UpdateManyIfExistAsync_WhenEntitiesDoNotExistInDatabase_ShouldDoNothing()
    {
        // Arrange  
        var dummiesToUpdate = GenerateDummies(1);

        // Act
        var dummiesReturned = await DbContext.Dummies.UpdateManyIfExistAsync<DummyEntity, Guid>(dummiesToUpdate);
        var dummyEntries = GetDummyEntries(EntityState.Modified);

        // Assert
        Assert.Empty(dummiesReturned);
        Assert.Empty(dummyEntries);
    }

    // AddOrAttachManyAsync
    [Fact]
    public async Task AddOrAttachManyAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<DummyEntity> dbSet = null!;
        var dummiesToAddOrUpdate = Enumerable.Empty<DummyEntity>();

        // Act
        async Task Action()
            => await dbSet.AddOrAttachManyAsync<DummyEntity, Guid>(dummiesToAddOrUpdate);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task AddOrAttachManyAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<DummyEntity> dummiesToAddOrAttach = null!;

        // Act
        async Task Action()
            => await DbContext.Dummies.AddOrAttachManyAsync<DummyEntity, Guid>(dummiesToAddOrAttach);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task AddOrAttachManyAsync_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var dummiesToAddOrAttach = GenerateDummies(1);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        async Task Action()
            => await DbContext.Dummies.AddOrAttachManyAsync<DummyEntity, Guid>(dummiesToAddOrAttach, cancellationTokenSource.Token);

        // Assert 
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task AddOrAttachManyAsync_WhenEntitiesDoNotExistInDatabase_ShouldSetEntitiesAsAddedAndReturnThem()
    {
        // Arrange 
        var dummiesToAdd = GenerateDummies(1);

        // Act
        var dummiesReturned = await DbContext.Dummies.AddOrAttachManyAsync<DummyEntity, Guid>(dummiesToAdd);
        var dummyEntries = GetDummyEntries(EntityState.Added);

        // Assert 
        Assert.Equivalent(dummiesToAdd, dummiesReturned);
        Assert.Equivalent(dummiesToAdd, dummyEntries.Select(entry => entry.Entity));
    }

    [Fact]
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
        Assert.Equivalent(dummiesToAttach, dummiesReturned);
        Assert.Equivalent(dummiesToAttach, dummyEntries.Select(entry => entry.Entity));
    }

    [Fact]
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
        Assert.Equivalent(dummiesToAddOrAttach, dummiesReturned);
        Assert.Equivalent(dummiesToAdd, addedEntries.Select(entry => entry.Entity));
        Assert.Equivalent(dummiesToAttach, mofidiedEntries.Select(entry => entry.Entity));
    }

    // AddOrUpdateManyAsync
    [Fact]
    public async Task AddOrUpdateManyAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<DummyEntity> dbSet = null!;
        var dummiesToAddOrUpdate = Enumerable.Empty<DummyEntity>();

        // Act
        async Task Action()
            => await dbSet.AddOrUpdateManyAsync<DummyEntity, Guid>(dummiesToAddOrUpdate);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task AddOrUpdateManyAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<DummyEntity> dummiesToAddOrUpdate = null!;

        // Act
        async Task Action()
            => await DbContext.Dummies.AddOrUpdateManyAsync<DummyEntity, Guid>(dummiesToAddOrUpdate);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task AddOrUpdateManyAsync_WhenCancellationIsRequested_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var dummiesToAddOrUpdate = GenerateDummies(1);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        async Task Action()
            => await DbContext.Dummies.AddOrUpdateManyAsync<DummyEntity, Guid>(dummiesToAddOrUpdate, cancellationTokenSource.Token);

        // Assert 
        await Assert.ThrowsAsync<TaskCanceledException>(Action);
    }

    [Fact]
    public async Task AddOrUpdateManyAsync_WhenEntitiesDoNotExistInDatabase_ShouldSetEntitiesAsAddedAndReturnThem()
    {
        // Arrange 
        var dummiesToAdd = GenerateDummies(1);

        // Act
        var dummiesReturned = await DbContext.Dummies.AddOrUpdateManyAsync<DummyEntity, Guid>(dummiesToAdd);
        var dummyEntries = GetDummyEntries(EntityState.Added);

        // Assert 
        Assert.Equivalent(dummiesToAdd, dummiesReturned);
        Assert.All(dummyEntries, entry => Assert.Equal(EntityState.Added, entry.State));
        Assert.Equivalent(dummiesToAdd, dummyEntries.Select(entry => entry.Entity));
    }

    [Fact]
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
        Assert.Equivalent(dummiesToUpdate, dummiesReturned);
        Assert.Equivalent(dummiesToUpdate, dummyEntries.Select(entry => entry.Entity));
    }

    [Fact]
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
        Assert.Equivalent(dummiesToAddOrUpdate, dummiesReturned);
        Assert.Equivalent(dummiesToAdd, addedEntries.Select(entry => entry.Entity));
        Assert.Equivalent(dummiesToUpdate, mofidiedEntries.Select(entry => entry.Entity));
    }


}


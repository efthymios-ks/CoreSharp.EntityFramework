using CoreSharp.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Extensions.Tests;

[TestFixture]
public sealed class DbSetExtensionsTests : AppDbContextTestsBase
{
    [Test]
    public async Task AddManyAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<Teacher> dbSet = null;
        var teachersToAdd = Enumerable.Empty<Teacher>();

        // Act
        Func<Task> action = () => dbSet.AddManyAsync(teachersToAdd);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddManyAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<Teacher> teachersToAdd = null;

        // Act
        Func<Task> action = () => AppDbContext.Teachers.AddManyAsync(teachersToAdd);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddManyAsync_WhenCalled_ShouldAddMultipleEntitiesToDatabase()
    {
        // Arrange
        const int toAddCount = 2;
        var teachersToAdd = GenerateTeachers(toAddCount);

        // Act
        var addedTeachers = await AppDbContext.Teachers.AddManyAsync(teachersToAdd);
        await AppDbContext.SaveChangesAsync();
        var afterAddCount = await AppDbContext.Teachers.CountAsync();

        // Assert
        addedTeachers.Should().NotBeNull();
        addedTeachers.Should().HaveCount(toAddCount);
        afterAddCount.Should().Be(toAddCount);
    }

    [Test]
    public async Task AddManyIfNotExistAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<Teacher> dbSet = null;
        var teachersToAdd = Enumerable.Empty<Teacher>();

        // Act
        Func<Task> action = () => dbSet.AddManyIfNotExistAsync(teachersToAdd);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddManyIfNotExistAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<Teacher> teachersToAdd = null;

        // Act
        Func<Task> action = () => AppDbContext.Teachers.AddManyIfNotExistAsync(teachersToAdd);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddManyIfNotExistAsync_WhenEntitiesDoNotExistInDatabase_ShouldAddNewEntities()
    {
        // Arrange
        const int toAddCount = 2;
        var teachersToAdd = GenerateTeachers(toAddCount);

        // Act
        var addedTeachers = await AppDbContext.Teachers.AddManyIfNotExistAsync(teachersToAdd);
        await AppDbContext.SaveChangesAsync();
        var afterAddCount = await AppDbContext.Teachers.CountAsync();

        // Assert
        addedTeachers.Should().NotBeNull();
        addedTeachers.Should().HaveCount(toAddCount);
        afterAddCount.Should().Be(toAddCount);
    }

    [Test]
    public async Task AddManyIfNotExistAsync_WhenEntitiesAlreadyExistInDatabase_ShouldNotAddEntities()
    {
        // Arrange  
        var existingTeacher = (await InsertTeachersAsync(1))[0];
        var teacherToAdd = GenerateTeacher();
        var teachersToTryAdd = new[]
        {
            existingTeacher,
            teacherToAdd
        };

        // Act
        var addedTeachers = await AppDbContext.Teachers.AddManyIfNotExistAsync(teachersToTryAdd);
        await AppDbContext.SaveChangesAsync();
        var afterAddCount = await AppDbContext.Teachers.CountAsync();

        // Assert
        addedTeachers.Should().NotBeNull();
        addedTeachers.Should().HaveCount(1);
        addedTeachers.Should().ContainEquivalentOf(teacherToAdd);
        afterAddCount.Should().Be(2);
    }

    [Test]
    public async Task UpdateManyAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<Teacher> dbSet = null;
        var teachers = Enumerable.Empty<Teacher>();

        // Act
        Func<Task> action = () => dbSet.UpdateManyAsync(teachers);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task UpdateManyAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<Teacher> teachersToUpdate = null;

        // Act
        Func<Task> action = () => AppDbContext.Teachers.UpdateManyAsync(teachersToUpdate);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task UpdateManyAsync_WhenEntitiesExistInDatabase_ShouldUpdateEntities()
    {
        // Arrange
        const int initialCount = 2;
        var existingTeachers = await InsertTeachersAsync(initialCount);
        existingTeachers.ForEach(teacher => teacher.Name = $"Updated {teacher.Name}");

        // Act
        var updatedTeachers = await AppDbContext.Teachers.UpdateManyAsync(existingTeachers);
        await AppDbContext.SaveChangesAsync();

        // Assert
        updatedTeachers.Should().NotBeNull();
        updatedTeachers.Should().HaveCount(initialCount);
        updatedTeachers.Should().BeEquivalentTo(existingTeachers);
    }

    [Test]
    public async Task UpdateManyIfExistAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<Teacher> dbSet = null;
        var teachersToUpdate = Enumerable.Empty<Teacher>();

        // Act
        Func<Task> action = () => dbSet.UpdateManyIfExistAsync(teachersToUpdate);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task UpdateManyIfExistAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<Teacher> teachersToUpdate = null;

        // Act
        Func<Task> action = () => AppDbContext.Teachers.UpdateManyIfExistAsync(teachersToUpdate);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task UpdateManyIfExistAsync_WhenEntitiesExistInDatabase_ShouldUpdateEntities()
    {
        // Arrange
        const int initialCount = 2;
        var existingTeachers = await InsertTeachersAsync(initialCount);
        existingTeachers.ForEach(teacher => teacher.Name = $"Updated {teacher.Name}");

        // Act
        var updatedTeachers = await AppDbContext.Teachers.UpdateManyIfExistAsync(existingTeachers);
        await AppDbContext.SaveChangesAsync();

        // Assert
        updatedTeachers.Should().NotBeNull();
        updatedTeachers.Should().HaveCount(initialCount);
        updatedTeachers.Should().BeEquivalentTo(existingTeachers);
    }

    [Test]
    public async Task UpdateManyIfExistAsync_WhenEntitiesDoNotExistInDatabase_ShouldNotUpdateEntities()
    {
        // Arrange  
        var existingTeacher = (await InsertTeachersAsync(1))[0];
        var nonExistingTeacher = GenerateTeacher();
        var teachersToUpdate = new[]
        {
            existingTeacher,
            nonExistingTeacher
        };

        // Act
        existingTeacher.Name = Guid.NewGuid().ToString();
        var updatedTeachers = await AppDbContext.Teachers.UpdateManyIfExistAsync(teachersToUpdate);
        await AppDbContext.SaveChangesAsync();

        // Assert
        updatedTeachers.Should().NotBeNull();
        updatedTeachers.Should().HaveCount(1);
        updatedTeachers.Should().ContainEquivalentOf(existingTeacher);
        updatedTeachers.Should().NotContainEquivalentOf(nonExistingTeacher);
    }

    [Test]
    public async Task AddOrUpdateManyAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<Teacher> dbSet = null;
        var teachersToAddOrUpdate = Enumerable.Empty<Teacher>();

        // Act
        Func<Task> action = () => dbSet.AddOrUpdateManyAsync(teachersToAddOrUpdate);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddOrUpdateManyAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<Teacher> teachersToAddOrUpdate = null;

        // Act
        Func<Task> action = () => AppDbContext.Teachers.AddOrUpdateManyAsync(teachersToAddOrUpdate);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddOrUpdateManyAsync_WhenEntitiesDoNotExistInDatabase_ShouldAddNewEntities()
    {
        // Arrange
        const int toAddCount = 2;
        var teachersToAdd = GenerateTeachers(toAddCount);

        // Act
        var addedTeachers = await AppDbContext.Teachers.AddOrUpdateManyAsync(teachersToAdd);
        await AppDbContext.SaveChangesAsync();

        // Assert
        addedTeachers.Should().NotBeNull();
        addedTeachers.Should().HaveCount(toAddCount);
        addedTeachers.Should().BeEquivalentTo(teachersToAdd);
    }

    [Test]
    public async Task AddOrUpdateManyAsync_WhenEntitiesExistInDatabase_ShouldUpdateExistingEntities()
    {
        // Arrange
        const int initialCount = 2;
        var existingTeachers = await InsertTeachersAsync(initialCount);
        existingTeachers.ForEach(teacher => teacher.Name = $"Updated {teacher.Name}");

        // Act
        var updatedTeachers = await AppDbContext.Teachers.AddOrUpdateManyAsync(existingTeachers);
        await AppDbContext.SaveChangesAsync();

        // Assert
        updatedTeachers.Should().NotBeNull();
        updatedTeachers.Should().HaveCount(initialCount);
        updatedTeachers.Should().BeEquivalentTo(existingTeachers);
    }

    [Test]
    public async Task AddOrUpdateManyAsync_WhenCalled_ShouldAddNewEntitiesAndUpdateExistingEntities()
    {
        // Arrange
        const int initialCount = 2;
        const int toAddCount = 2;
        var existingTeachers = await InsertTeachersAsync(initialCount);
        existingTeachers.ForEach(teacher => teacher.Name = $"Updated {teacher.Name}");
        var teachersToAdd = GenerateTeachers(toAddCount);
        var teachersToAddOrUpdate = teachersToAdd.Concat(existingTeachers);

        // Act
        var addOrUpdatedTeachers = await AppDbContext.Teachers.AddOrUpdateManyAsync(teachersToAddOrUpdate);
        await AppDbContext.SaveChangesAsync();

        // Assert
        addOrUpdatedTeachers.Should().NotBeNull();
        addOrUpdatedTeachers.Should().HaveCount(initialCount + toAddCount);
        addOrUpdatedTeachers.Should().BeEquivalentTo(teachersToAddOrUpdate);
    }

    [Test]
    public async Task AttachManyAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<Teacher> dbSet = null;
        var teachersToAttach = GenerateTeachers(2);

        // Act
        Func<Task> action = () => dbSet.AttachManyAsync(teachersToAttach);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AttachManyAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        IEnumerable<Teacher> teachersToAttach = null;

        // Act
        Func<Task> action = async () => await AppDbContext.Teachers.AttachManyAsync(teachersToAttach);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AttachManyAsync_WhenCalled_ShouldAttachEntitiesToDbContext()
    {
        // Arrange
        const int toAttachCount = 2;
        var teachersToAttach = GenerateTeachers(toAttachCount);

        // Act
        var attachedTeachers = await AppDbContext.Teachers.AttachManyAsync(teachersToAttach);
        await AppDbContext.SaveChangesAsync();

        // Assert 
        attachedTeachers.Should().NotBeNull();
        attachedTeachers.Should().HaveCount(toAttachCount);
        attachedTeachers.Should().BeEquivalentTo(attachedTeachers);
        foreach (var teacher in teachersToAttach)
        {
            var teacherEntityEntry = AppDbContext.Entry(teacher);
            teacherEntityEntry.State.Should().Be(EntityState.Unchanged);
        }
    }

    [Test]
    public async Task AttachManyIfExistAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<Teacher> dbSet = null;
        var teachersToAttach = GenerateTeachers(1);

        // Act
        Func<Task> action = () => dbSet.AttachManyIfExistAsync(teachersToAttach);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AttachManyIfExistAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<Teacher> teachersToAttach = null;

        // Act
        Func<Task> action = () => AppDbContext.Teachers.AttachManyIfExistAsync(teachersToAttach);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AttachManyIfExistAsync_WhenEntitiesExistInDatabase_ShouldAttachExistingEntities()
    {
        // Arrange
        const int initialCount = 2;
        var existingTeachers = await InsertTeachersAsync(initialCount);

        // Act
        var attachedTeachers = await AppDbContext.Teachers.AttachManyIfExistAsync(existingTeachers);
        await AppDbContext.SaveChangesAsync();

        // Assert
        attachedTeachers.Should().NotBeNull();
        attachedTeachers.Should().HaveCount(initialCount);
        foreach (var teacher in attachedTeachers)
        {
            var teacherEntityEntry = AppDbContext.Entry(teacher);
            teacherEntityEntry.State.Should().Be(EntityState.Unchanged);
        }
    }

    [Test]
    public async Task AttachManyIfExistAsync_WhenEntitiesDoNotExistInDatabase_ShouldNotAttachEntities()
    {
        // Arrange
        var teachersToAttach = GenerateTeachers(2);

        // Act
        var attachedTeachers = await AppDbContext.Teachers.AttachManyIfExistAsync(teachersToAttach);
        await AppDbContext.SaveChangesAsync();
        var countAfterAttach = await AppDbContext.Teachers.CountAsync();

        // Assert
        attachedTeachers.Should().NotBeNull();
        attachedTeachers.Should().HaveCount(0);
        countAfterAttach.Should().Be(0);
    }

    [Test]
    public async Task AddOrAttachManyAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<Teacher> dbSet = null;
        var teachersToAddOrAttach = GenerateTeachers(1);

        // Act
        Func<Task> action = () => dbSet.AddOrAttachManyAsync(teachersToAddOrAttach);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddOrAttachManyAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<Teacher> teachersToAddOrAttach = null;

        // Act
        Func<Task> action = () => AppDbContext.Teachers.AddOrAttachManyAsync(teachersToAddOrAttach);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddOrAttachManyAsync_WhenEntitiesDoNotExistInDatabase_ShouldAddNewEntities()
    {
        // Arrange
        const int toAddCount = 2;
        var teachersToAdd = GenerateTeachers(toAddCount);

        // Act
        var addedTeachers = await AppDbContext.Teachers.AddOrAttachManyAsync(teachersToAdd);
        await AppDbContext.SaveChangesAsync();

        // Assert
        addedTeachers.Should().NotBeNull();
        addedTeachers.Should().HaveCount(toAddCount);
        addedTeachers.Should().BeEquivalentTo(teachersToAdd);
    }

    [Test]
    public async Task AddOrAttachManyAsync_WhenEntitiesExistInDatabase_ShouldAttachExistingEntities()
    {
        // Arrange
        const int initialCount = 2;
        var existingTeachers = await InsertTeachersAsync(initialCount);

        // Act
        var attachedTeachers = await AppDbContext.Teachers.AddOrAttachManyAsync(existingTeachers);
        await AppDbContext.SaveChangesAsync();

        // Assert
        attachedTeachers.Should().NotBeNull();
        attachedTeachers.Should().HaveCount(initialCount);
        foreach (var teacher in attachedTeachers)
        {
            var teacherEntityEntry = AppDbContext.Entry(teacher);
            teacherEntityEntry.State.Should().Be(EntityState.Unchanged);
        }
    }

    [Test]
    public async Task AddOrAttachManyAsync_WhenCalled_ShouldAddNewEntitiesAndUpdateExistingEntities()
    {
        // Arrange
        const int initialCount = 5;
        const int toAddCount = 2;
        var existingTeachers = await InsertTeachersAsync(initialCount);
        existingTeachers.ForEach(teacher => teacher.Name = $"Updated {teacher.Name}");
        var teachersToAdd = GenerateTeachers(toAddCount);
        var teachersToAddOrAttach = teachersToAdd.Concat(existingTeachers);

        // Act
        var addedOrAttachedTeachers = await AppDbContext.Teachers.AddOrAttachManyAsync(teachersToAddOrAttach);
        await AppDbContext.SaveChangesAsync();

        // Assert
        addedOrAttachedTeachers.Should().NotBeNull();
        addedOrAttachedTeachers.Should().HaveCount(initialCount + toAddCount);
        AppDbContext.Teachers.Should().BeEquivalentTo(teachersToAddOrAttach);
    }

    [Test]
    public async Task RemoveByKeyAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<Teacher> dbSet = null;
        var teacherToRemove = GenerateTeacher();

        // Act
        Func<Task> action = () => dbSet.RemoveByKeyAsync(teacherToRemove.Id);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task RemoveByKeyAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        object teacherIdToRemove = null;

        // Act
        Func<Task> action = () => AppDbContext.Teachers.RemoveByKeyAsync(teacherIdToRemove);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task RemoveByKeyAsync_WhenKeyExists_ShouldRemoveEntityFromDbContext()
    {
        // Arrange 
        var existingTeacher = (await InsertTeachersAsync(1))[0];

        // Act
        var teachersRemovedCount = await AppDbContext.Teachers.RemoveByKeyAsync(existingTeacher.Id);
        await AppDbContext.SaveChangesAsync();

        // Assert
        teachersRemovedCount.Should().Be(1);
    }

    [Test]
    public async Task RemoveByKeyAsync_WhenKeyNotFound_ShouldNotRemoveEntityFromDbContext()
    {
        // Arrange 
        var teachersBeforeRemove = await InsertTeachersAsync(1);

        // Act
        var teachersRemovedCount = await AppDbContext.Teachers.RemoveByKeyAsync(Guid.NewGuid());
        await AppDbContext.SaveChangesAsync();
        var teachersAfterRemove = await AppDbContext.Teachers.ToArrayAsync();

        // Assert
        teachersRemovedCount.Should().Be(0);
        teachersAfterRemove.Should().BeEquivalentTo(teachersBeforeRemove);
    }

    [Test]
    public async Task RemoveManyAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<Teacher> dbSet = null;
        var teachersToRemove = GenerateTeachers(1);

        // Act
        Func<Task> action = () => dbSet.RemoveManyAsync(teachersToRemove);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task RemoveManyAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<Teacher> teachersToRemove = null;

        // Act
        Func<Task> action = () => AppDbContext.Teachers.RemoveManyAsync(teachersToRemove);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task RemoveManyAsync_WhenCalled_ShouldRemoveEntitiesFromDbContext()
    {
        // Arrange
        const int initialCount = 5;
        const int toRemoveCount = 3;
        var existingTeachers = await InsertTeachersAsync(initialCount);
        var teachersToRemove = existingTeachers.Take(toRemoveCount);

        // Act
        await AppDbContext.Teachers.RemoveManyAsync(teachersToRemove);
        await AppDbContext.SaveChangesAsync();
        var teachersAfterRemove = await AppDbContext.Teachers.ToArrayAsync();

        // Assert
        teachersAfterRemove.Should().HaveCount(initialCount - toRemoveCount);
        teachersAfterRemove.Should().NotContain(teachersToRemove);
    }
}


using CoreSharp.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Extensions.Tests;

[TestFixture]
public sealed class DbSetExtensionsTests : AppDbContextTestsBase
{
    [Test]
    public async Task AddManyAsync_ShouldThrowArgumentNullException_WhenDbSetIsNull()
    {
        // Arrange
        DbSet<Teacher> dbSet = null;
        var teachersToAdd = Enumerable.Empty<Teacher>();

        // Act
        Func<Task> action = async () => await dbSet.AddManyAsync(teachersToAdd);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddManyAsync_ShouldThrowArgumentNullException_WhenEntitiesIsNull()
    {
        // Arrange
        IEnumerable<Teacher> teachersToAdd = null;

        // Act
        Func<Task> action = async () => await AppDbContext.Teachers.AddManyAsync(teachersToAdd);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddManyAsync_ShouldAddMultipleEntitiesToDatabase_WhenCalled()
    {
        // Arrange
        const int toAddCount = 2;
        var teachersToAdd = CreateTeachers(toAddCount);

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
        Func<Task> action = async () => await dbSet.AddManyIfNotExistAsync(teachersToAdd);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddManyIfNotExistAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<Teacher> teachersToAdd = null;

        // Act
        Func<Task> action = async () => await AppDbContext.Teachers.AddManyIfNotExistAsync(teachersToAdd);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddManyIfNotExistAsync_WhenEntitiesDoNotExistInDatabase_ShouldAddNewEntities()
    {
        // Arrange
        const int toAddCount = 2;
        var teachersToAdd = CreateTeachers(toAddCount);

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
        const int initialCount = 2;
        var existingTeachers = await PopulateTeachersAsync(initialCount);

        // Act
        var addedTeachers = await AppDbContext.Teachers.AddManyIfNotExistAsync(existingTeachers);
        await AppDbContext.SaveChangesAsync();
        var afterAddCount = await AppDbContext.Teachers.CountAsync();

        // Assert
        addedTeachers.Should().NotBeNull();
        addedTeachers.Should().HaveCount(0);
        afterAddCount.Should().Be(initialCount);
    }

    [Test]
    public async Task UpdateManyAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<Teacher> dbSet = null;
        var teachers = Enumerable.Empty<Teacher>();

        // Act
        Func<Task> action = async () => await dbSet.UpdateManyAsync(teachers);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task UpdateManyAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<Teacher> teachersToUpdate = null;

        // Act
        Func<Task> action = async () => await AppDbContext.Teachers.UpdateManyAsync(teachersToUpdate);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task UpdateManyAsync_WhenEntitiesExistInDatabase_ShouldUpdateEntities()
    {
        // Arrange
        const int initialCount = 2;
        var existingTeachers = await PopulateTeachersAsync(initialCount);
        existingTeachers.ForEach(teacher => teacher.Name = $"Updated {teacher.Name}");

        // Act
        var updatedTeachers = await AppDbContext.Teachers.UpdateManyAsync(existingTeachers);
        await AppDbContext.SaveChangesAsync();

        // Assert
        updatedTeachers.Should().NotBeNull();
        updatedTeachers.Should().HaveCount(initialCount);
        updatedTeachers.Should().BeEquivalentTo(existingTeachers,
            options => options.Excluding(teacher => teacher.Id));
    }

    [Test]
    public async Task UpdateManyIfExistAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<Teacher> dbSet = null;
        var teachersToUpdate = Enumerable.Empty<Teacher>();

        // Act
        Func<Task> action = async () => await dbSet.UpdateManyIfExistAsync(teachersToUpdate);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task UpdateManyIfExistAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<Teacher> teachersToUpdate = null;

        // Act
        Func<Task> action = async () => await AppDbContext.Teachers.UpdateManyIfExistAsync(teachersToUpdate);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task UpdateManyIfExistAsync_WhenEntitiesExistInDatabase_ShouldUpdateEntities()
    {
        // Arrange
        const int initialCount = 2;
        var existingTeachers = await PopulateTeachersAsync(initialCount);
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
        const int initialCount = 2;
        var teachers = CreateTeachers(initialCount);

        // Act
        var updatedTeachers = await AppDbContext.Teachers.UpdateManyIfExistAsync(teachers);
        await AppDbContext.SaveChangesAsync();

        // Assert
        updatedTeachers.Should().NotBeNull();
        updatedTeachers.Should().HaveCount(0);
    }

    [Test]
    public async Task AddOrUpdateManyAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<Teacher> dbSet = null;
        var teachersToAddOrUpdate = Enumerable.Empty<Teacher>();

        // Act
        Func<Task> action = async () => await dbSet.AddOrUpdateManyAsync(teachersToAddOrUpdate);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddOrUpdateManyAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<Teacher> teachersToAddOrUpdate = null;

        // Act
        Func<Task> action = async () => await AppDbContext.Teachers.AddOrUpdateManyAsync(teachersToAddOrUpdate);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddOrUpdateManyAsync_WhenEntitiesDoNotExistInDatabase_ShouldAddNewEntities()
    {
        // Arrange
        const int toAddCount = 2;
        var teachersToAdd = CreateTeachers(toAddCount);

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
        var existingTeachers = await PopulateTeachersAsync(initialCount);
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
        var existingTeachers = await PopulateTeachersAsync(initialCount);
        existingTeachers.ForEach(teacher => teacher.Name = $"Updated {teacher.Name}");
        var teachersToAdd = CreateTeachers(toAddCount);
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
        var teachersToAttach = CreateTeachers(2);

        // Act
        Func<Task> action = async () => await dbSet.AttachManyAsync(teachersToAttach);

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
        var teachersToAttach = CreateTeachers(toAttachCount);

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
        var teachersToAttach = CreateTeachers(1);

        // Act
        Func<Task> action = async () => await dbSet.AttachManyIfExistAsync(teachersToAttach);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AttachManyIfExistAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<Teacher> teachersToAttach = null;

        // Act
        Func<Task> action = async () => await AppDbContext.Teachers.AttachManyIfExistAsync(teachersToAttach);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AttachManyIfExistAsync_WhenEntitiesExistInDatabase_ShouldAttachExistingEntities()
    {
        // Arrange
        const int initialCount = 2;
        var existingTeachers = await PopulateTeachersAsync(initialCount);
        existingTeachers.ForEach(teacher => teacher.Name = $"Updated {teacher.Name}");

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
        var teachersToAttach = CreateTeachers(2);

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
        var teachersToAddOrAttach = CreateTeachers(1);

        // Act
        Func<Task> action = async () => await dbSet.AddOrAttachManyAsync(teachersToAddOrAttach);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddOrAttachManyAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<Teacher> teachersToAddOrAttach = null;

        // Act
        Func<Task> action = async () => await AppDbContext.Teachers.AddOrAttachManyAsync(teachersToAddOrAttach);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddOrAttachManyAsync_WhenEntitiesDoNotExistInDatabase_ShouldAddNewEntities()
    {
        // Arrange
        const int toAddCount = 2;
        var teachersToAdd = CreateTeachers(toAddCount);

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
        var existingTeachers = await PopulateTeachersAsync(initialCount);
        existingTeachers.ForEach(teacher => teacher.Name = $"Updated {teacher.Name}");

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
        var existingTeachers = await PopulateTeachersAsync(initialCount);
        existingTeachers.ForEach(teacher => teacher.Name = $"Updated {teacher.Name}");
        var teachersToAdd = CreateTeachers(toAddCount);
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
    public async Task RemoveManyAsync_WhenDbSetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        DbSet<Teacher> dbSet = null;
        var teachersToRemove = CreateTeachers(1);

        // Act
        Func<Task> action = async () => await dbSet.RemoveManyAsync(teachersToRemove);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task RemoveManyAsync_WhenEntitiesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<Teacher> teachersToRemove = null;

        // Act
        Func<Task> action = async () => await AppDbContext.Teachers.RemoveManyAsync(teachersToRemove);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task RemoveManyAsync_WhenCalled_ShouldRemoveEntitiesFromDbContext()
    {
        // Arrange
        const int initialCount = 5;
        const int toRemoveCount = 3;
        var existingTeachers = await PopulateTeachersAsync(initialCount);
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


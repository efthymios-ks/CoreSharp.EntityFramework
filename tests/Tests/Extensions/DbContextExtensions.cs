using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Extensions.Tests;

[TestFixture]
public sealed class DbContextExtensionsTests : AppDbContextTestsBase
{
    [Test]
    public async Task RollbackAsync_WhenNoChanges_ShouldNotThrowException()
    {
        // Act
        Func<Task> action = () => AppDbContext.RollbackAsync();

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Test]
    public async Task RollbackAsync_WhenEntityAdded_ShouldRemoveAddedEntity()
    {
        // Arrange
        var teacher = CreateTeacher();
        var initialCount = await AppDbContext.Teachers.CountAsync();

        // Act
        await AppDbContext.Teachers.AddAsync(teacher);
        await AppDbContext.RollbackAsync();
        var finalCount = await AppDbContext.Teachers.CountAsync();

        // Assert
        finalCount.Should().Be(initialCount);
    }

    [Test]
    public async Task RollbackAsync_WhenEntityModified_ShouldRestoreEntityToOriginalState()
    {
        // Arrange
        await PopulateTeachersAsync(1);
        var teacher = await AppDbContext.Teachers.FirstOrDefaultAsync();
        var originalName = teacher.Name;

        // Act
        teacher.Name = Guid.NewGuid().ToString();
        await AppDbContext.RollbackAsync();
        var modifiedTeacher = await AppDbContext.Teachers.FindAsync(teacher.Id);

        // Assert 
        modifiedTeacher.Should().NotBeNull();
        modifiedTeacher.Name.Should().Be(originalName);
    }

    [Test]
    public async Task RollbackAsync_WhenEntityDeleted_ShouldRestoreEntityToOriginalState()
    {
        // Arrange
        await PopulateTeachersAsync(1);
        var teacher = await AppDbContext.Teachers.FirstOrDefaultAsync();
        var initialCount = await AppDbContext.Teachers.CountAsync();

        // Act
        AppDbContext.Teachers.Remove(teacher);
        await AppDbContext.RollbackAsync();
        var finalCount = await AppDbContext.Teachers.CountAsync();

        // Assert 
        finalCount.Should().Be(initialCount);
        var restoredTeacher = await AppDbContext.Teachers.FindAsync(teacher.Id);
        restoredTeacher.Should().NotBeNull();
    }
}

using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CoreSharp.EntityFramework.DbContexts.Abstracts.Tests;

[TestFixture]
public sealed class AuditableDbContextBaseTests : AppDbContextTestsBase
{
    // Methods 
    [Test]
    public async Task SaveChanges_WhenEntityAdded_ShouldSaveChangesAndUpdateDataChanges()
    {
        // Arrange 
        var teacher = new Teacher
        {
            Name = "Efthymios"
        };

        // Act
        AppDbContext.Teachers.Add(teacher);
        AppDbContext.SaveChanges();

        // Assert
        var change = await AppDbContext
            .DataChanges
            .OrderBy(entity => entity.DateCreatedUtc)
            .LastOrDefaultAsync();
        change.Should().NotBeNull();
        change.TableName.Should().Be("Teachers");
        change.Action.Should().Be(EntityState.Added.ToString());
        change.Keys.Should().Be(JsonSerializer.Serialize(new { teacher.Id }));
    }

    [Test]
    public async Task SaveChangesAsync_WhenEntityChanged_ShouldSaveChangesAndUpdateDataChangesAsync()
    {
        // Arrange 
        var existingTeacher = (await InsertTeachersAsync(1))[0];

        // Act
        existingTeacher.Name = "Efthymios K.";
        await AppDbContext.SaveChangesAsync();

        // Assert
        var change = await AppDbContext
            .DataChanges
            .OrderBy(entity => entity.DateCreatedUtc)
            .LastOrDefaultAsync();
        change.Should().NotBeNull();
        change.TableName.Should().Be("Teachers");
        change.Action.Should().Be(EntityState.Modified.ToString());
        change.Keys.Should().Be(JsonSerializer.Serialize(new { existingTeacher.Id }));
    }

    [Test]
    public async Task SaveChangesAsync_WhenUniqueEntityAdded_ShouldGenerateAndReturnPrimaryKey()
    {
        // Arrange 
        var teacher = new Teacher
        {
            Name = $"Teacher_{DateTime.Now.ToFileTimeUtc()}"
        };

        // Act 
        await AppDbContext.Teachers.AddAsync(teacher);
        await AppDbContext.SaveChangesAsync();

        // Assert
        teacher.Id.Should().NotBe(Guid.Empty);
    }

    [Test]
    public async Task SaveChangesAsync_WhenAuditableEntityAdded_ShouldSetDateCreatedUtc()
    {
        // Arrange 
        var teacher = new Teacher
        {
            Name = $"Teacher_{DateTime.Now.ToFileTimeUtc()}"
        };

        // Act 
        var dateCreatedUtcBeforeAdding = teacher.DateCreatedUtc;
        await AppDbContext.Teachers.AddAsync(teacher);
        await AppDbContext.SaveChangesAsync();
        var dateCreatedUtcAfterAdding = teacher.DateCreatedUtc;

        // Assert
        dateCreatedUtcAfterAdding.Should().BeOnOrAfter(dateCreatedUtcBeforeAdding);
    }

    [Test]
    public async Task SaveChangesAsync_WhenAuditableEntityAdded_ShouldNotChangeDateModifiedUtc()
    {
        // Arrange 
        var teacher = new Teacher
        {
            Name = $"Teacher_{DateTime.Now.ToFileTimeUtc()}"
        };

        // Act 
        var dateModifiedUtcBeforeAdding = teacher.DateModifiedUtc;
        await AppDbContext.Teachers.AddAsync(teacher);
        await AppDbContext.SaveChangesAsync();
        var dateModifiedUtcAfterAdding = teacher.DateModifiedUtc;

        // Assert
        dateModifiedUtcAfterAdding.Should().Be(dateModifiedUtcBeforeAdding);
    }

    [Test]
    public async Task SaveChangesAsync_WhenAuditableEntityUpdated_ShouldNotChangeDateCreatedUtc()
    {
        // Arrange 
        var teacher = new Teacher
        {
            Name = $"Teacher_{DateTime.Now.ToFileTimeUtc()}",
            TeacherType = TeacherType.Elementary
        };
        await AppDbContext.Teachers.AddAsync(teacher);
        await AppDbContext.SaveChangesAsync();

        // Act 
        var dateCreatedUtcBeforeUpdating = teacher.DateCreatedUtc;
        teacher.TeacherType = TeacherType.MiddleSchool;
        AppDbContext.Teachers.Update(teacher);
        await AppDbContext.SaveChangesAsync();
        var dateCreatedUtcAfterUpdating = teacher.DateCreatedUtc;

        // Assert 
        dateCreatedUtcAfterUpdating.Should().Be(dateCreatedUtcBeforeUpdating);
    }

    [Test]
    public async Task SaveChangesAsync_WhenAuditableEntityUpdated_ShouldSetDateModifiedUtc()
    {
        // Arrange 
        var teacher = new Teacher
        {
            Name = $"Teacher_{DateTime.Now.ToFileTimeUtc()}",
            TeacherType = TeacherType.Elementary
        };
        await AppDbContext.Teachers.AddAsync(teacher);
        await AppDbContext.SaveChangesAsync();

        // Act 
        var dateModifiedUtcBeforeUpdating = teacher.DateModifiedUtc ?? DateTime.MinValue.ToUniversalTime();
        teacher.TeacherType = TeacherType.MiddleSchool;
        AppDbContext.Teachers.Update(teacher);
        await AppDbContext.SaveChangesAsync();
        var dateModifiedUtcAfterUpdating = teacher.DateModifiedUtc;

        // Assert
        dateModifiedUtcAfterUpdating.Should().NotBeNull();
        dateModifiedUtcAfterUpdating.Should().BeOnOrAfter(dateModifiedUtcBeforeUpdating);
    }
}
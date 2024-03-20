using CoreSharp.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CoreSharp.EntityFramework.DbContexts.Abstracts;

[TestFixture]
public sealed class AuditableDbContextBaseTests : AppDbContextTestsBase
{
    // Methods
    [Test]
    public void DataChanges_WhenCalled_ShouldBeInitialized()
    {
        // Act & Assert
        AppDbContext.DataChanges.Should().NotBeNull();
        AppDbContext.DataChanges.Should().BeAssignableTo<DbSet<EntityChange>>();
    }

    [Test]
    public async Task SaveChanges_WhenEntitiesAdded_ShouldSaveChangesAndUpdateDataChanges()
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
        var changedEntity = await AppDbContext
            .DataChanges
            .OrderBy(entity => entity.DateCreatedUtc)
            .LastOrDefaultAsync();
        changedEntity.Should().NotBeNull();
        changedEntity.TableName.Should().Be("Teachers");
        changedEntity.Action.Should().Be(EntityState.Added.ToString());
        changedEntity.Keys.Should().Be(JsonSerializer.Serialize(new { teacher.Id }));
    }

    [Test]
    public async Task SaveChangesAsync_WhenCalledAndEntitiesChanged_ShouldSaveChangesAndUpdateDataChangesAsync()
    {
        // Arrange 
        var teacher = new Teacher { Name = "Efthymios" };

        // Act
        AppDbContext.Teachers.Add(teacher);
        await AppDbContext.SaveChangesAsync();
        teacher.Name = "Efthymios K.";
        AppDbContext.Attach(teacher);
        await AppDbContext.SaveChangesAsync();

        // Assert
        var changedEntity = await AppDbContext
            .DataChanges
            .OrderBy(entity => entity.DateCreatedUtc)
            .LastOrDefaultAsync();
        changedEntity.Should().NotBeNull();
        changedEntity.TableName.Should().Be("Teachers");
        changedEntity.Action.Should().Be(EntityState.Modified.ToString());
        changedEntity.Keys.Should().Be(JsonSerializer.Serialize(new { teacher.Id }));
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
}
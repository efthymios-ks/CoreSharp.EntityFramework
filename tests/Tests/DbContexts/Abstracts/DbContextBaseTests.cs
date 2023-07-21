namespace Tests.DbContexts.Common;

[TestFixture]
public class DbContextBaseTests : DbContextTestsBase
{
    // Methods
    [Test]
    public async Task SaveChangesAsync_IUniqueEntityAdded_PrimaryKeyIsGeneratedAndReturned()
    {
        // Arrange 
        var teacher = new Teacher
        {
            Name = $"Teacher_{DateTime.Now.ToFileTimeUtc()}"
        };

        // Act 
        await DbContext.Teachers.AddAsync(teacher);
        await DbContext.SaveChangesAsync();

        // Assert
        teacher.Id.Should().NotBe(Guid.Empty);
    }

    [Test]
    public async Task SaveChangesAsync_ITrackableEntityAdded_SetsDateCreatedUtc()
    {
        // Arrange 
        var teacher = new Teacher
        {
            Name = $"Teacher_{DateTime.Now.ToFileTimeUtc()}"
        };

        // Act 
        var dateCreatedUtcBeforeAdding = teacher.DateCreatedUtc;
        await DbContext.Teachers.AddAsync(teacher);
        await DbContext.SaveChangesAsync();
        var dateCreatedUtcAfterAdding = teacher.DateCreatedUtc;

        // Assert
        dateCreatedUtcAfterAdding.Should().BeOnOrAfter(dateCreatedUtcBeforeAdding);
    }

    [Test]
    public async Task SaveChangesAsync_ITrackableEntityAdded_DateModifiedUtcIsUntouched()
    {
        // Arrange 
        var teacher = new Teacher
        {
            Name = $"Teacher_{DateTime.Now.ToFileTimeUtc()}"
        };

        // Act 
        var dateModifiedUtcBeforeAdding = teacher.DateModifiedUtc;
        await DbContext.Teachers.AddAsync(teacher);
        await DbContext.SaveChangesAsync();
        var dateModifiedUtcAfterAdding = teacher.DateModifiedUtc;

        // Assert
        dateModifiedUtcAfterAdding.Should().Be(dateModifiedUtcBeforeAdding);
    }

    [Test]
    public async Task SaveChangesAsync_ITrackableEntityUpdated_SetsDateModifiedUtc()
    {
        // Arrange 
        var teacher = new Teacher
        {
            Name = $"Teacher_{DateTime.Now.ToFileTimeUtc()}",
            TeacherType = TeacherType.Elementary
        };
        await DbContext.Teachers.AddAsync(teacher);
        await DbContext.SaveChangesAsync();

        // Act 
        var dateModifiedUtcBeforeUpdating = teacher.DateModifiedUtc ?? DateTime.MinValue.ToUniversalTime();
        teacher.TeacherType = TeacherType.MiddleSchool;
        DbContext.Teachers.Update(teacher);
        await DbContext.SaveChangesAsync();
        var dateModifiedUtcAfterUpdating = teacher.DateModifiedUtc;

        // Assert
        dateModifiedUtcAfterUpdating.Should().NotBeNull();
        dateModifiedUtcAfterUpdating.Should().BeOnOrAfter(dateModifiedUtcBeforeUpdating);
    }

    [Test]
    public async Task SaveChangesAsync_ITrackableEntityUpdated_DateCreatedUtcIsUntouched()
    {
        // Arrange 
        var teacher = new Teacher
        {
            Name = $"Teacher_{DateTime.Now.ToFileTimeUtc()}",
            TeacherType = TeacherType.Elementary
        };
        await DbContext.Teachers.AddAsync(teacher);
        await DbContext.SaveChangesAsync();

        // Act 
        var dateCreatedUtcBeforeUpdating = teacher.DateCreatedUtc;
        teacher.TeacherType = TeacherType.MiddleSchool;
        DbContext.Teachers.Update(teacher);
        await DbContext.SaveChangesAsync();
        var dateCreatedUtcAfterUpdating = teacher.DateCreatedUtc;

        // Assert 
        dateCreatedUtcAfterUpdating.Should().Be(dateCreatedUtcBeforeUpdating);
    }
}
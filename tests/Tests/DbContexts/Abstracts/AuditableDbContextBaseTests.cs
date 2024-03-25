using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Tests.DbContexts.Abstracts;

// TODO: Revisit all 
[TestFixture]
public sealed class AuditableDbContextBaseTests : DummyDbContextTestsBase
{
    // Methods 
    [Test]
    public async Task SaveChanges_WhenEntityAdded_ShouldSaveChangesAndUpdateDataChanges()
    {
        // Arrange 
        var dummyToAdd = GenerateDummy();
        DbContext.Dummies.Add(dummyToAdd);

        // Act
        DbContext.SaveChanges();

        // Assert
        var change = await DbContext
            .DataChanges
            .OrderBy(entity => entity.DateCreatedUtc)
            .LastOrDefaultAsync();
        change.Should().NotBeNull();
        change.TableName.Should().Be("Dummies");
        change.Action.Should().Be(EntityState.Added.ToString());
        change.Keys.Should().Be(JsonSerializer.Serialize(new { dummyToAdd.Id }));
    }

    [Test]
    public async Task SaveChangesAsync_WhenEntityChanged_ShouldSaveChangesAndUpdateDataChangesAsync()
    {
        // Arrange 
        var dummyToUpdate = await PreloadDummyAsync();

        // Act
        dummyToUpdate.Name = Guid.NewGuid().ToString();
        await DbContext.SaveChangesAsync();

        // Assert
        var change = await DbContext
            .DataChanges
            .OrderBy(entity => entity.DateCreatedUtc)
            .LastOrDefaultAsync();
        change.Should().NotBeNull();
        change.TableName.Should().Be("Dummies");
        change.Action.Should().Be(EntityState.Modified.ToString());
        change.Keys.Should().Be(JsonSerializer.Serialize(new { dummyToUpdate.Id }));
    }

    [Test]
    public async Task SaveChangesAsync_WhenUniqueEntityAdded_ShouldGenerateAndReturnPrimaryKey()
    {
        // Arrange 
        var dummy = GenerateDummy();
        await DbContext.Dummies.AddAsync(dummy);

        // Act 
        await DbContext.SaveChangesAsync();

        // Assert
        dummy.Id.Should().NotBe(Guid.Empty);
    }

    [Test]
    public async Task SaveChangesAsync_WhenAuditableEntityAdded_ShouldSetDateCreatedUtc()
    {
        // Arrange 
        var dummyToAdd = GenerateDummy();

        // Act 
        var dateCreatedUtcBeforeAdding = dummyToAdd.DateCreatedUtc;
        await DbContext.Dummies.AddAsync(dummyToAdd);
        await DbContext.SaveChangesAsync();
        var dateCreatedUtcAfterAdding = dummyToAdd.DateCreatedUtc;

        // Assert
        dateCreatedUtcAfterAdding.Should().BeOnOrAfter(dateCreatedUtcBeforeAdding);
    }

    [Test]
    public async Task SaveChangesAsync_WhenAuditableEntityAdded_ShouldNotChangeDateModifiedUtc()
    {
        // Arrange 
        var dummyToAdd = GenerateDummy();

        // Act 
        var dateModifiedUtcBeforeAdding = dummyToAdd.DateModifiedUtc;
        await DbContext.Dummies.AddAsync(dummyToAdd);
        await DbContext.SaveChangesAsync();
        var dateModifiedUtcAfterAdding = dummyToAdd.DateModifiedUtc;

        // Assert
        dateModifiedUtcAfterAdding.Should().Be(dateModifiedUtcBeforeAdding);
    }

    [Test]
    public async Task SaveChangesAsync_WhenAuditableEntityUpdated_ShouldNotChangeDateCreatedUtc()
    {
        // Arrange 
        var dummyToUpdate = await PreloadDummyAsync();

        // Act 
        var dateCreatedUtcBeforeUpdating = dummyToUpdate.DateCreatedUtc;
        dummyToUpdate.Name = Guid.NewGuid().ToString();
        DbContext.Dummies.Update(dummyToUpdate);
        await DbContext.SaveChangesAsync();
        var dateCreatedUtcAfterUpdating = dummyToUpdate.DateCreatedUtc;

        // Assert 
        dateCreatedUtcAfterUpdating.Should().Be(dateCreatedUtcBeforeUpdating);
    }

    [Test]
    public async Task SaveChangesAsync_WhenAuditableEntityUpdated_ShouldSetDateModifiedUtc()
    {
        // Arrange 
        var dummyToUpdate = await PreloadDummyAsync();

        // Act 
        var dateModifiedUtcBeforeUpdating = dummyToUpdate.DateModifiedUtc ?? DateTime.MinValue.ToUniversalTime();
        dummyToUpdate.Name = Guid.NewGuid().ToString();
        DbContext.Dummies.Update(dummyToUpdate);
        await DbContext.SaveChangesAsync();
        var dateModifiedUtcAfterUpdating = dummyToUpdate.DateModifiedUtc;

        // Assert
        dateModifiedUtcAfterUpdating.Should().NotBeNull();
        dateModifiedUtcAfterUpdating.Should().BeOnOrAfter(dateModifiedUtcBeforeUpdating);
    }
}
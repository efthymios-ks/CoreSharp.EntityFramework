using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Tests.Internal.Database.Models;

namespace Tests.DbContexts.Abstracts;

[TestFixture]
public sealed class AuditableDbContextBaseTests : DummyDbContextTestsBase
{
    // Methods 
    [Test]
    public async Task SaveChanges_WhenEntityAdded_ShouldSaveChangesAndUpdateDataChanges()
    {
        // Arrange 
        var dummy = GenerateDummy();

        // Act
        DbContext.Dummies.Add(dummy);
        DbContext.SaveChanges();

        // Assert
        var change = await DbContext
            .DataChanges
            .OrderBy(entity => entity.DateCreatedUtc)
            .LastOrDefaultAsync();
        change.Should().NotBeNull();
        change.TableName.Should().Be("Dummies");
        change.Action.Should().Be(EntityState.Added.ToString());
        change.Keys.Should().Be(JsonSerializer.Serialize(new { dummy.Id }));
    }

    [Test]
    public async Task SaveChangesAsync_WhenEntityChanged_ShouldSaveChangesAndUpdateDataChangesAsync()
    {
        // Arrange 
        var existingDummy = (await PreloadDummiesAsync(1))[0];

        // Act
        existingDummy.Name = Guid.NewGuid().ToString();
        await DbContext.SaveChangesAsync();

        // Assert
        var change = await DbContext
            .DataChanges
            .OrderBy(entity => entity.DateCreatedUtc)
            .LastOrDefaultAsync();
        change.Should().NotBeNull();
        change.TableName.Should().Be("Dummies");
        change.Action.Should().Be(EntityState.Modified.ToString());
        change.Keys.Should().Be(JsonSerializer.Serialize(new { existingDummy.Id }));
    }

    [Test]
    public async Task SaveChangesAsync_WhenUniqueEntityAdded_ShouldGenerateAndReturnPrimaryKey()
    {
        // Arrange 
        var dummy = new DummyEntity
        {
            Name = Guid.NewGuid().ToString()
        };

        // Act 
        await DbContext.Dummies.AddAsync(dummy);
        await DbContext.SaveChangesAsync();

        // Assert
        dummy.Id.Should().NotBe(Guid.Empty);
    }

    [Test]
    public async Task SaveChangesAsync_WhenAuditableEntityAdded_ShouldSetDateCreatedUtc()
    {
        // Arrange 
        var dummy = new DummyEntity
        {
            Name = Guid.NewGuid().ToString()
        };

        // Act 
        var dateCreatedUtcBeforeAdding = dummy.DateCreatedUtc;
        await DbContext.Dummies.AddAsync(dummy);
        await DbContext.SaveChangesAsync();
        var dateCreatedUtcAfterAdding = dummy.DateCreatedUtc;

        // Assert
        dateCreatedUtcAfterAdding.Should().BeOnOrAfter(dateCreatedUtcBeforeAdding);
    }

    [Test]
    public async Task SaveChangesAsync_WhenAuditableEntityAdded_ShouldNotChangeDateModifiedUtc()
    {
        // Arrange 
        var dummy = new DummyEntity
        {
            Name = Guid.NewGuid().ToString()
        };

        // Act 
        var dateModifiedUtcBeforeAdding = dummy.DateModifiedUtc;
        await DbContext.Dummies.AddAsync(dummy);
        await DbContext.SaveChangesAsync();
        var dateModifiedUtcAfterAdding = dummy.DateModifiedUtc;

        // Assert
        dateModifiedUtcAfterAdding.Should().Be(dateModifiedUtcBeforeAdding);
    }

    [Test]
    public async Task SaveChangesAsync_WhenAuditableEntityUpdated_ShouldNotChangeDateCreatedUtc()
    {
        // Arrange 
        var existingDummy = (await PreloadDummiesAsync(1))[0];

        // Act 
        var dateCreatedUtcBeforeUpdating = existingDummy.DateCreatedUtc;
        existingDummy.Name = Guid.NewGuid().ToString();
        DbContext.Dummies.Update(existingDummy);
        await DbContext.SaveChangesAsync();
        var dateCreatedUtcAfterUpdating = existingDummy.DateCreatedUtc;

        // Assert 
        dateCreatedUtcAfterUpdating.Should().Be(dateCreatedUtcBeforeUpdating);
    }

    [Test]
    public async Task SaveChangesAsync_WhenAuditableEntityUpdated_ShouldSetDateModifiedUtc()
    {
        // Arrange 
        var existingDummy = (await PreloadDummiesAsync(1))[0];

        // Act 
        var dateModifiedUtcBeforeUpdating = existingDummy.DateModifiedUtc ?? DateTime.MinValue.ToUniversalTime();
        existingDummy.Name = Guid.NewGuid().ToString();
        DbContext.Dummies.Update(existingDummy);
        await DbContext.SaveChangesAsync();
        var dateModifiedUtcAfterUpdating = existingDummy.DateModifiedUtc;

        // Assert
        dateModifiedUtcAfterUpdating.Should().NotBeNull();
        dateModifiedUtcAfterUpdating.Should().BeOnOrAfter(dateModifiedUtcBeforeUpdating);
    }
}
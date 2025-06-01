using CoreSharp.EntityFramework.Tests.Internal.Database;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CoreSharp.EntityFramework.Tests.DbContexts.Abstracts;

[Collection(nameof(DummySqlServerCollection))]
public sealed class AuditableDbContextBaseTests(DummySqlServerContainer sqlContainer)
    : DummySqlServerTestsBase(sqlContainer)
{
    [Fact]
    public async Task SaveChanges_WhenEntityAdded_ShouldSaveChangesAndUpdateDataChanges()
    {
        // Arrange 
        var dummyToAdd = GenerateDummy();
        DummyDbContext.Dummies.Add(dummyToAdd);

        // Act
        DummyDbContext.SaveChanges();

        // Assert
        var change = await DummyDbContext
            .DataChanges
            .OrderBy(entity => entity.DateCreatedUtc)
            .LastOrDefaultAsync();
        Assert.NotNull(change);
        Assert.Equal("Dummies", change!.TableName);
        Assert.Equal(EntityState.Added.ToString(), change.Action);
        Assert.Equal(JsonSerializer.Serialize(new { dummyToAdd.Id }), change.Keys);
    }

    [Fact]
    public async Task SaveChangesAsync_WhenEntityChanged_ShouldSaveChangesAndUpdateDataChangesAsync()
    {
        // Arrange 
        var dummyToUpdate = await PreloadDummyAsync();

        // Act
        dummyToUpdate.Name = Guid.NewGuid().ToString();
        await DummyDbContext.SaveChangesAsync();

        // Assert
        var change = await DummyDbContext
            .DataChanges
            .OrderBy(entity => entity.DateCreatedUtc)
            .LastOrDefaultAsync();
        Assert.NotNull(change);
        Assert.Equal("Dummies", change!.TableName);
        Assert.Equal(EntityState.Modified.ToString(), change.Action);
        Assert.Equal(JsonSerializer.Serialize(new { dummyToUpdate.Id }), change.Keys);
    }

    [Fact]
    public async Task SaveChangesAsync_WhenUniqueEntityAdded_ShouldGenerateAndReturnPrimaryKey()
    {
        // Arrange 
        var dummy = GenerateDummy();
        await DummyDbContext.Dummies.AddAsync(dummy);

        // Act 
        await DummyDbContext.SaveChangesAsync();

        // Assert
        Assert.NotEqual(Guid.Empty, dummy.Id);
    }

    [Fact]
    public async Task SaveChangesAsync_WhenAuditableEntityAdded_ShouldSetDateCreatedUtc()
    {
        // Arrange 
        var dummyToAdd = GenerateDummy();

        // Act 
        var dateCreatedUtcBeforeAdding = dummyToAdd.DateCreatedUtc;
        await DummyDbContext.Dummies.AddAsync(dummyToAdd);
        await DummyDbContext.SaveChangesAsync();
        var dateCreatedUtcAfterAdding = dummyToAdd.DateCreatedUtc;

        // Assert
        Assert.True(dateCreatedUtcAfterAdding >= dateCreatedUtcBeforeAdding);
    }

    [Fact]
    public async Task SaveChangesAsync_WhenAuditableEntityAdded_ShouldNotChangeDateModifiedUtc()
    {
        // Arrange 
        var dummyToAdd = GenerateDummy();

        // Act 
        var dateModifiedUtcBeforeAdding = dummyToAdd.DateModifiedUtc;
        await DummyDbContext.Dummies.AddAsync(dummyToAdd);
        await DummyDbContext.SaveChangesAsync();
        var dateModifiedUtcAfterAdding = dummyToAdd.DateModifiedUtc;

        // Assert
        Assert.Equal(dateModifiedUtcBeforeAdding, dateModifiedUtcAfterAdding);
    }

    [Fact]
    public async Task SaveChangesAsync_WhenAuditableEntityUpdated_ShouldNotChangeDateCreatedUtc()
    {
        // Arrange 
        var dummyToUpdate = await PreloadDummyAsync();

        // Act 
        var dateCreatedUtcBeforeUpdating = dummyToUpdate.DateCreatedUtc;
        dummyToUpdate.Name = Guid.NewGuid().ToString();
        DummyDbContext.Dummies.Update(dummyToUpdate);
        await DummyDbContext.SaveChangesAsync();
        var dateCreatedUtcAfterUpdating = dummyToUpdate.DateCreatedUtc;

        // Assert 
        Assert.Equal(dateCreatedUtcBeforeUpdating, dateCreatedUtcAfterUpdating);
    }

    [Fact]
    public async Task SaveChangesAsync_WhenAuditableEntityUpdated_ShouldSetDateModifiedUtc()
    {
        // Arrange 
        var dummyToUpdate = await PreloadDummyAsync();

        // Act 
        var dateModifiedUtcBeforeUpdating = dummyToUpdate.DateModifiedUtc ?? DateTime.MinValue.ToUniversalTime();
        dummyToUpdate.Name = Guid.NewGuid().ToString();
        DummyDbContext.Dummies.Update(dummyToUpdate);
        await DummyDbContext.SaveChangesAsync();
        var dateModifiedUtcAfterUpdating = dummyToUpdate.DateModifiedUtc;

        // Assert
        Assert.NotNull(dateModifiedUtcAfterUpdating);
        Assert.True(dateModifiedUtcAfterUpdating >= dateModifiedUtcBeforeUpdating);
    }
}

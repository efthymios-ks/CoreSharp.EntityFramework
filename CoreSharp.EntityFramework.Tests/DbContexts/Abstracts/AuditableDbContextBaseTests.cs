using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CoreSharp.EntityFramework.Tests.DbContexts.Abstracts;

[Collection(nameof(SharedSqlServerCollection))]
public sealed class AuditableDbContextBaseTests(SharedSqlServerContainer sqlContainer)
    : SharedSqlServerTestsBase(sqlContainer)
{
    [Fact]
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
        await DbContext.SaveChangesAsync();

        // Assert
        var change = await DbContext
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
        await DbContext.Dummies.AddAsync(dummy);

        // Act 
        await DbContext.SaveChangesAsync();

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
        await DbContext.Dummies.AddAsync(dummyToAdd);
        await DbContext.SaveChangesAsync();
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
        await DbContext.Dummies.AddAsync(dummyToAdd);
        await DbContext.SaveChangesAsync();
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
        DbContext.Dummies.Update(dummyToUpdate);
        await DbContext.SaveChangesAsync();
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
        DbContext.Dummies.Update(dummyToUpdate);
        await DbContext.SaveChangesAsync();
        var dateModifiedUtcAfterUpdating = dummyToUpdate.DateModifiedUtc;

        // Assert
        Assert.NotNull(dateModifiedUtcAfterUpdating);
        Assert.True(dateModifiedUtcAfterUpdating >= dateModifiedUtcBeforeUpdating);
    }
}

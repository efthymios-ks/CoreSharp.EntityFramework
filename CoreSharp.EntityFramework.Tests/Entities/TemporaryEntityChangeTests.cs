using CoreSharp.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Tests.Entities;

[Collection(nameof(SharedSqlServerCollection))]
public sealed class TemporaryEntityChangeTests(SharedSqlServerContainer sqlContainer)
    : SharedSqlServerTestsBase(sqlContainer)
{
    [Fact]
    public void Constructor_WhenEntryIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        static void Action()
            => _ = new TemporaryEntityChange(entry: null!);

        // Assert
        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task Constructor_WhenCalled_ShouldInitializeProperties()
    {
        // Arrange
        var dummyToAdd = GenerateDummy();
        await DbContext.Dummies.AddAsync(dummyToAdd);
        var change = GetDummyEntry(EntityState.Added)!;

        // Act
        var temporaryChange = new TemporaryEntityChange(change);

        // Assert 
        Assert.Equal("Dummies", temporaryChange.TableName);
        Assert.NotNull(temporaryChange.Keys);
        Assert.NotNull(temporaryChange.PreviousState);
        Assert.NotNull(temporaryChange.NewState);
        Assert.NotNull(temporaryChange.TemporaryProperties);
    }

    [Fact]
    public async Task Constructor_WhenEntryAdded_ShouldSetActionToAdded()
    {
        // Arrange
        var dummyToAdd = GenerateDummy();
        await DbContext.Dummies.AddAsync(dummyToAdd);
        var dummyEntry = GetDummyEntry(EntityState.Added)!;

        // Act
        var temporaryChange = new TemporaryEntityChange(dummyEntry);

        // Assert 
        Assert.Equal(EntityState.Added.ToString(), temporaryChange.Action);
    }

    [Fact]
    public async Task Constructor_WhenEntryUpdated_ShouldSetActionToModified()
    {
        // Arrange
        var dummyToModify = await PreloadDummyAsync();
        dummyToModify.Name = Guid.NewGuid().ToString();
        var dummyEntry = GetDummyEntry(EntityState.Modified)!;

        // Act
        var temporaryChange = new TemporaryEntityChange(dummyEntry);

        // Assert 
        Assert.Equal(EntityState.Modified.ToString(), temporaryChange.Action);
    }

    [Fact]
    public async Task Constructor_WhenEntryRemoved_ShouldSetActionToDeleted()
    {
        // Arrange
        var dummyToRemove = await PreloadDummyAsync();
        DbContext.Dummies.Remove(dummyToRemove);
        var dummyEntry = GetDummyEntry(EntityState.Deleted)!;

        // Act
        var temporaryChange = new TemporaryEntityChange(dummyEntry);

        // Assert 
        Assert.Equal(EntityState.Deleted.ToString(), temporaryChange.Action);
    }

    [Fact]
    public async Task Constructor_WhenEntryDetached_ShouldSetActionToDetached()
    {
        // Arrange
        await PreloadDummyAsync();
        var dummyEntry = GetDummyEntry(EntityState.Unchanged)!;
        dummyEntry.State = EntityState.Detached;

        // Act
        var temporaryChange = new TemporaryEntityChange(dummyEntry);

        // Assert 
        Assert.Equal(EntityState.Detached.ToString(), temporaryChange.Action);
    }

    [Fact]
    public async Task ToEntityChange_WhenDictionariesAreEmpty_ShouldSerializeEmptyProperties()
    {
        // Arrange
        await PreloadDummyAsync();
        var dummyEntry = GetDummyEntry(EntityState.Unchanged)!;
        var temporaryChange = new TemporaryEntityChange(dummyEntry);

        // Act
        var entityChange = temporaryChange.ToEntityChange();

        // Assert
        Assert.NotNull(entityChange);
        Assert.Equal("Dummies", entityChange.TableName);
        Assert.Equal(EntityState.Unchanged.ToString(), entityChange.Action);
        Assert.Null(entityChange.Keys);
        Assert.Null(entityChange.PreviousState);
        Assert.Null(entityChange.NewState);
    }

    [Fact]
    public async Task ToEntityChange_WhenDictionariesHaveValues_ShouldSerializeProperties()
    {
        // Arrange
        await PreloadDummyAsync();
        var dummyEntry = GetDummyEntry(EntityState.Unchanged)!;
        var temporaryChange = new TemporaryEntityChange(dummyEntry);
        temporaryChange.Keys.Add("Key1", "Value1");
        temporaryChange.PreviousState.Add("Property1", "OldValue");
        temporaryChange.NewState.Add("Property1", "NewValue");

        // Act
        var entityChange = temporaryChange.ToEntityChange();

        // Assert
        Assert.NotNull(entityChange);
        Assert.Equal("Dummies", entityChange.TableName);
        Assert.Equal(EntityState.Unchanged.ToString(), entityChange.Action);
        Assert.Equal("{\"Key1\":\"Value1\"}", entityChange.Keys);
        Assert.Equal("{\"Property1\":\"OldValue\"}", entityChange.PreviousState);
        Assert.Equal("{\"Property1\":\"NewValue\"}", entityChange.NewState);
    }
}

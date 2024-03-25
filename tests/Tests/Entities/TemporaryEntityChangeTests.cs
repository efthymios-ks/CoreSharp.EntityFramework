using CoreSharp.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace Tests.Entities;

[TestFixture]
public sealed class TemporaryEntityChangeTests : DummyDbContextTestsBase
{
    [Test]
    public void Constructor_WhenEntryIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        Action action = () => _ = new TemporaryEntityChange(entry: null);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    [Test]
    public async Task Constructor_WhenCalled_ShouldInitializeProperties()
    {
        // Arrange
        var dummyToAdd = GenerateDummy();
        await DbContext.Dummies.AddAsync(dummyToAdd);
        var change = GetDummyEntry(EntityState.Added);

        // Act
        var temporaryChange = new TemporaryEntityChange(change);

        // Assert 
        temporaryChange.TableName.Should().Be("Dummies");
        temporaryChange.Keys.Should().NotBeNull();
        temporaryChange.PreviousState.Should().NotBeNull();
        temporaryChange.NewState.Should().NotBeNull();
        temporaryChange.TemporaryProperties.Should().NotBeNull();
    }

    [Test]
    public async Task Constructor_WhenEntryAdded_ShouldSetActionToAdded()
    {
        // Arrange
        var dummyToAdd = GenerateDummy();
        await DbContext.Dummies.AddAsync(dummyToAdd);
        var dummyEntry = GetDummyEntry(EntityState.Added);

        // Act
        var temporaryChange = new TemporaryEntityChange(dummyEntry);

        // Assert 
        temporaryChange.Action.Should().Be(EntityState.Added.ToString());
    }

    [Test]
    public async Task Constructor_WhenEntryUpdated_ShouldSetActionToModified()
    {
        // Arrange
        var dummyToModify = await PreloadDummyAsync();
        dummyToModify.Name = Guid.NewGuid().ToString();
        var dummyEntry = GetDummyEntry(EntityState.Modified);

        // Act
        var temporaryChange = new TemporaryEntityChange(dummyEntry);

        // Assert 
        temporaryChange.Action.Should().Be(EntityState.Modified.ToString());
    }

    [Test]
    public async Task Constructor_WhenEntryRemoved_ShouldSetActionToDeleted()
    {
        // Arrange
        var dummyToRemove = await PreloadDummyAsync();
        DbContext.Dummies.Remove(dummyToRemove);
        var dummyEntry = GetDummyEntry(EntityState.Deleted);

        // Act
        var temporaryChange = new TemporaryEntityChange(dummyEntry);

        // Assert 
        temporaryChange.Action.Should().Be(EntityState.Deleted.ToString());
    }

    [Test]
    public async Task Constructor_WhenEntryDetached_ShouldSetActionToDetached()
    {
        // Arrange
        await PreloadDummyAsync();
        var dummyEntry = GetDummyEntry(EntityState.Unchanged);
        dummyEntry.State = EntityState.Detached;

        // Act
        var temporaryChange = new TemporaryEntityChange(dummyEntry);

        // Assert 
        temporaryChange.Action.Should().Be(EntityState.Detached.ToString());
    }

    [Test]
    public async Task ToEntityChange_WhenDictionariesAreEmpty_ShouldSerializeEmptyProperties()
    {
        // Arrange
        await PreloadDummyAsync();
        var dummyEntry = GetDummyEntry(EntityState.Unchanged);
        var temporaryChange = new TemporaryEntityChange(dummyEntry);

        // Act
        var entityChange = temporaryChange.ToEntityChange();

        // Assert
        entityChange.Should().NotBeNull();
        entityChange.TableName.Should().Be("Dummies");
        entityChange.Action.Should().Be(EntityState.Unchanged.ToString());
        entityChange.Keys.Should().BeNullOrEmpty();
        entityChange.PreviousState.Should().BeNullOrEmpty();
        entityChange.NewState.Should().BeNullOrEmpty();
    }

    [Test]
    public async Task ToEntityChange_WhenDictionariesHaveValues_ShouldSerializeProperties()
    {
        // Arrange
        await PreloadDummyAsync();
        var dummyEntry = GetDummyEntry(EntityState.Unchanged);
        var temporaryChange = new TemporaryEntityChange(dummyEntry);
        temporaryChange.Keys.Add("Key1", "Value1");
        temporaryChange.PreviousState.Add("Property1", "OldValue");
        temporaryChange.NewState.Add("Property1", "NewValue");

        // Act
        var entityChange = temporaryChange.ToEntityChange();

        // Assert
        entityChange.Should().NotBeNull();
        entityChange.TableName.Should().Be("Dummies");
        entityChange.Action.Should().Be(EntityState.Unchanged.ToString());
        entityChange.Keys.Should().Be(/*lang=json,strict*/ "{\"Key1\":\"Value1\"}");
        entityChange.PreviousState.Should().Be(/*lang=json,strict*/ "{\"Property1\":\"OldValue\"}");
        entityChange.NewState.Should().Be(/*lang=json,strict*/ "{\"Property1\":\"NewValue\"}");
    }
}

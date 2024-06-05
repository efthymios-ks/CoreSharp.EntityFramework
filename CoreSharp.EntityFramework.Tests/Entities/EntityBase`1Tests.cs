using CoreSharp.EntityFramework.Entities.Abstracts;
using CoreSharp.EntityFramework.Entities.Interfaces;

namespace Tests.Entities;

[TestFixture]
public sealed class EntityBase_1_Tests
{
    // Methods 
    [Test]
    public void Id_Set_ShouldSetBaseId()
    {
        // Arrange
        var entity = new DummyEntity
        {
            Id = Guid.NewGuid()
        };

        // Act
        var baseId = ((IUniqueEntity)entity).Id;

        // Assert
        baseId.Should().Be(entity.Id);
    }

    [Test]
    public void Id_Get_ShouldGetBaseId()
    {
        // Arrange
        var entity = new DummyEntity();
        var baseId = Guid.NewGuid();
        ((IUniqueEntity)entity).Id = baseId;

        // Act
        var id = entity.Id;

        // Assert
        id.Should().Be(baseId);
    }

    [Test]
    public void ToString_WhenIdNotSet_ShouldReturnDefaultForType()
    {
        // Arrange  
        var entity = new DummyEntity();
        var expected = Guid.Empty.ToString();

        // Act
        var entityAsString = entity.ToString();

        // Assert
        entityAsString.Should().Be(expected);
    }

    [Test]
    public void ToString_WhenNotOverriden_ShouldReturnId()
    {
        // Arrange 
        var entity = new DummyEntity
        {
            Id = Guid.NewGuid()
        };
        var expected = entity.Id.ToString();

        // Act
        var entityAsString = entity.ToString();

        // Assert
        entityAsString.Should().Be(expected);
    }

    private sealed class DummyEntity : EntityBase<Guid>
    {
    }
}

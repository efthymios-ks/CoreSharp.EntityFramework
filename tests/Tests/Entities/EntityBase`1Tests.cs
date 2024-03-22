using CoreSharp.EntityFramework.Entities.Abstracts;
using CoreSharp.EntityFramework.Entities.Interfaces;

namespace Tests.Entities;

[TestFixture]
public sealed class EntityBase_1_Tests
{
    // Methods 
    [Test]
    public void Id_Setter_ShouldSetBaseId()
    {
        // Arrange
        var entity = new GenericTestEntity
        {
            Id = Guid.NewGuid()
        };

        // Act
        var baseId = (entity as IUniqueEntity)!.Id;

        // Assert
        baseId.Should().Be(entity.Id);
    }

    [Test]
    public void Id_Getter_ShouldGetBaseId()
    {
        // Arrange
        var entity = new GenericTestEntity();
        var baseId = Guid.NewGuid();
        (entity as IUniqueEntity).Id = baseId;

        // Act
        var id = entity.Id;

        // Assert
        id.Should().Be(baseId);
    }

    [Test]
    public void ToString_WhenNotOverriden_ShouldReturnId()
    {
        // Arrange 
        var entity = new GenericTestEntity
        {
            Id = Guid.NewGuid()
        };
        var expected = entity.Id.ToString();

        // Act
        var entityAsString = entity.ToString();

        // Assert
        entityAsString.Should().Be(expected);
    }

    private sealed class GenericTestEntity : EntityBase<Guid>
    {
    }
}
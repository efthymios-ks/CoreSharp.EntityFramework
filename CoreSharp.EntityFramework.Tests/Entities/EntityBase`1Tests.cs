using CoreSharp.EntityFramework.Entities.Abstracts;
using CoreSharp.EntityFramework.Entities.Interfaces;

namespace CoreSharp.EntityFramework.Tests.Entities;

public sealed class EntityBase_1_Tests
{
    // Methods 
    [Fact]
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
        Assert.Equal(entity.Id, baseId);
    }

    [Fact]
    public void Id_Get_ShouldGetBaseId()
    {
        // Arrange
        var entity = new DummyEntity();
        var baseId = Guid.NewGuid();
        ((IUniqueEntity)entity).Id = baseId;

        // Act
        var id = entity.Id;

        // Assert
        Assert.Equal(baseId, id);
    }

    [Fact]
    public void ToString_WhenIdNotSet_ShouldReturnDefaultForType()
    {
        // Arrange  
        var entity = new DummyEntity();
        const string expected = "00000000-0000-0000-0000-000000000000";

        // Act
        var entityAsString = entity.ToString();

        // Assert
        Assert.Equal(expected, entityAsString);
    }

    [Fact]
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
        Assert.Equal(expected, entityAsString);
    }

    private sealed class DummyEntity : EntityBase<Guid>
    {
    }
}

using CoreSharp.EntityFramework.Entities;
using Tests.Internal.Models;

namespace Tests.Entities;

[TestFixture]
public sealed class EnumShadowEntityTests
{
    [Test]
    public void Constructor_WhenCalled_ShouldNotThrowException()
    {
        // Act
        Action act = () => _ = new EnumShadowEntity<DummyEnumeration>();

        // Assert
        act.Should().NotThrow();
    }

    [Test]
    public void Constructor_WhenCalledWithArgument_ShouldSetProperties()
    {
        // Arrange
        var value = DummyEnumeration.Value1;

        // Act
        var entity = new EnumShadowEntity<DummyEnumeration>(value);

        // Assert
        entity.Value.Should().Be(value);
        entity.Name.Should().Be(value.ToString());
    }
}


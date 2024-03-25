using CoreSharp.EntityFramework.Entities;

namespace Tests.Entities;

[TestFixture]
public sealed class EntityChangeTests
{
    [Test]
    public void DateCreatedUtc_Setter_WhenLocalDateTime_ShouldSetToUtc()
    {
        // Arrange
        var entity = new EntityChange();
        var localDateTime = new DateTime(2024, 3, 22, 12, 0, 0, DateTimeKind.Local);

        // Act
        entity.DateCreatedUtc = localDateTime;

        // Assert
        entity.DateCreatedUtc.Kind.Should().Be(DateTimeKind.Utc);
        entity.DateCreatedUtc.Should().Be(localDateTime.ToUniversalTime());
    }

    [Test]
    public void DateCreatedUtc_Setter_WhenUtcDateTime_ShouldSetToUtc()
    {
        // Arrange
        var entity = new EntityChange();
        var utcDateTime = new DateTime(2024, 3, 22, 12, 0, 0, DateTimeKind.Utc);

        // Act
        entity.DateCreatedUtc = utcDateTime;

        // Assert
        entity.DateCreatedUtc.Kind.Should().Be(DateTimeKind.Utc);
        entity.DateCreatedUtc.Should().Be(utcDateTime);
    }

    [Test]
    public void DateCreatedUtc_Setter_WhenUndefinedDateTime_ShouldSetToUtc()
    {
        // Arrange
        var entity = new EntityChange();
        var undefinedDateTime = new DateTime(2024, 3, 22, 12, 0, 0, DateTimeKind.Unspecified);

        // Act
        entity.DateCreatedUtc = undefinedDateTime;

        // Assert
        entity.DateCreatedUtc.Kind.Should().Be(DateTimeKind.Utc);
        entity.DateCreatedUtc.Should().Be(undefinedDateTime.ToUniversalTime());
    }
}

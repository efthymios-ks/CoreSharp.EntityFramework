using CoreSharp.EntityFramework.Entities;

namespace CoreSharp.EntityFramework.Tests.Entities;

public sealed class EntityChangeTests
{
    [Fact]
    public void DateCreatedUtc_Setter_WhenDateTimeIsLocal_ShouldSetToUtc()
    {
        // Arrange
        var entity = new EntityChange();
        var localDateTime = new DateTime(2024, 3, 22, 12, 0, 0, DateTimeKind.Local);

        // Act
        entity.DateCreatedUtc = localDateTime;

        // Assert
        Assert.Equal(DateTimeKind.Utc, entity.DateCreatedUtc.Kind);
        Assert.Equal(localDateTime.ToUniversalTime(), entity.DateCreatedUtc);
    }

    [Fact]
    public void DateCreatedUtc_Setter_WhenDateTimeIsUtc_ShouldSetToUtc()
    {
        // Arrange
        var entity = new EntityChange();
        var utcDateTime = new DateTime(2024, 3, 22, 12, 0, 0, DateTimeKind.Utc);

        // Act
        entity.DateCreatedUtc = utcDateTime;

        // Assert
        Assert.Equal(DateTimeKind.Utc, entity.DateCreatedUtc.Kind);
        Assert.Equal(utcDateTime, entity.DateCreatedUtc);
    }

    [Fact]
    public void DateCreatedUtc_Setter_WhenDateTimeisUnspecified_ShouldSetToUtc()
    {
        // Arrange
        var entity = new EntityChange();
        var undefinedDateTime = new DateTime(2024, 3, 22, 12, 0, 0, DateTimeKind.Unspecified);

        // Act
        entity.DateCreatedUtc = undefinedDateTime;

        // Assert
        Assert.Equal(DateTimeKind.Utc, entity.DateCreatedUtc.Kind);
        Assert.Equal(undefinedDateTime.ToUniversalTime(), entity.DateCreatedUtc);
    }
}

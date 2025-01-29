using CoreSharp.EntityFramework.Entities.Abstracts;
using CoreSharp.EntityFramework.Entities.Interfaces;

namespace CoreSharp.EntityFramework.Tests.Entities;

public sealed class EntityBaseTests
{
    [Fact]
    public void DateCreatedUtc_Setter_WhenDateTimeIsLocal_ShouldSetToUtc()
    {
        // Arrange
        var entity = new DummyEntity();
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
        var entity = new DummyEntity();
        var utcDateTime = new DateTime(2024, 3, 22, 12, 0, 0, DateTimeKind.Utc);

        // Act
        entity.DateCreatedUtc = utcDateTime;

        // Assert
        Assert.Equal(DateTimeKind.Utc, entity.DateCreatedUtc.Kind);
        Assert.Equal(utcDateTime, entity.DateCreatedUtc);
    }

    [Fact]
    public void DateCreatedUtc_Setter_WhenDateTimeIsUnspecified_ShouldSetToUtc()
    {
        // Arrange
        var entity = new DummyEntity();
        var unspecifiedDateTime = new DateTime(2024, 3, 22, 12, 0, 0, DateTimeKind.Unspecified);

        // Act
        entity.DateCreatedUtc = unspecifiedDateTime;

        // Assert
        Assert.Equal(DateTimeKind.Utc, entity.DateCreatedUtc.Kind);
        Assert.Equal(unspecifiedDateTime.ToUniversalTime(), entity.DateCreatedUtc);
    }

    [Fact]
    public void DateModifiedUtc_Setter_WhenNull_ShouldSetToNull()
    {
        // Arrange
        var entity = new DummyEntity
        {
            DateModifiedUtc = DateTime.UtcNow
        };

        // Act
        entity.DateModifiedUtc = null;

        // Assert
        Assert.Null(entity.DateModifiedUtc);
    }

    [Fact]
    public void DateModifiedUtc_Setter_WhenDateTimeIsLocal_ShouldSetToUtc()
    {
        // Arrange
        var entity = new DummyEntity();
        var localDateTime = new DateTime(2024, 3, 22, 12, 0, 0, DateTimeKind.Local);

        // Act
        entity.DateModifiedUtc = localDateTime;

        // Assert
        Assert.NotNull(entity.DateModifiedUtc);
        Assert.Equal(DateTimeKind.Utc, entity.DateModifiedUtc.Value.Kind);
        Assert.Equal(localDateTime.ToUniversalTime(), entity.DateModifiedUtc);
    }

    [Fact]
    public void DateModifiedUtc_Setter_WhenDateTimeIsUtc_ShouldSetToUtc()
    {
        // Arrange
        var entity = new DummyEntity();
        var utcDateTime = new DateTime(2024, 3, 22, 12, 0, 0, DateTimeKind.Utc);

        // Act
        entity.DateModifiedUtc = utcDateTime;

        // Assert
        Assert.NotNull(entity.DateModifiedUtc);
        Assert.Equal(DateTimeKind.Utc, entity.DateModifiedUtc.Value.Kind);
        Assert.Equal(utcDateTime, entity.DateModifiedUtc);
    }

    [Fact]
    public void DateModifiedUtc_Setter_WhenDateTimeIsUnspecified_ShouldSetToUtc()
    {
        // Arrange
        var entity = new DummyEntity();
        var undefinedDateTime = new DateTime(2024, 3, 22, 12, 0, 0, DateTimeKind.Unspecified);

        // Act
        entity.DateModifiedUtc = undefinedDateTime;

        // Assert
        Assert.NotNull(entity.DateModifiedUtc);
        Assert.Equal(DateTimeKind.Utc, entity.DateModifiedUtc.Value.Kind);
        Assert.Equal(undefinedDateTime.ToUniversalTime(), entity.DateModifiedUtc);
    }

    [Fact]
    public void DateCreatedUtc_WhenSerializedToJsonWithJsonNet_ShouldUseFormatO()
    {
        // Arrange
        const string dateAsJson = "2022-12-01T12:30:45.1234567Z";
        var date = DateTime.ParseExact(dateAsJson, "O", CultureInfo.InvariantCulture);
        var entity = new DummyEntity
        {
            DateCreatedUtc = date
        };
        var expectedDate = $"\"{dateAsJson}\"";

        // Act 
        var serializedDate = JsonNet.JsonConvert.SerializeObject(entity.DateCreatedUtc);

        // Assert 
        Assert.Equal(expectedDate, serializedDate);
    }

    [Fact]
    public void DateCreatedUtc_WhenSerializedToJsonUsingTextJson_ShouldUseFormatO()
    {
        // Arrange
        const string dateAsJson = "2022-12-01T12:30:45.1234567Z";
        var date = DateTime.ParseExact(dateAsJson, "O", CultureInfo.InvariantCulture);
        var entity = new DummyEntity
        {
            DateCreatedUtc = date
        };
        var expectedDate = $"\"{dateAsJson}\"";

        // Act 
        var serializedDate = TextJson.JsonSerializer.Serialize(entity.DateCreatedUtc);

        // Assert 
        Assert.Equal(expectedDate, serializedDate);
    }

    [Fact]
    public void DateModifiedUtc_WhenSerializedToJsonUsingJsonNet_ShouldUseFormatO()
    {
        // Arrange
        const string dateAsJson = "2022-12-01T12:30:45.1234567Z";
        var date = DateTime.ParseExact(dateAsJson, "O", CultureInfo.InvariantCulture);
        var entity = new DummyEntity
        {
            DateModifiedUtc = date
        };
        var expectedDate = $"\"{dateAsJson}\"";

        // Act 
        var serializedDate = JsonNet.JsonConvert.SerializeObject(entity.DateModifiedUtc);

        // Assert 
        Assert.Equal(expectedDate, serializedDate);
    }

    [Fact]
    public void DateModifiedUtc_WhenSerializedToJsonUsingTextJson_ShouldUseFormatO()
    {
        // Arrange
        const string dateAsJson = "2022-12-01T12:30:45.1234567Z";
        var date = DateTime.ParseExact(dateAsJson, "O", CultureInfo.InvariantCulture);
        var entity = new DummyEntity
        {
            DateModifiedUtc = date
        };
        var expectedDate = $"\"{dateAsJson}\"";

        // Act 
        var serializedDate = TextJson.JsonSerializer.Serialize(entity.DateModifiedUtc);

        // Assert 
        Assert.Equal(expectedDate, serializedDate);
    }

    [Fact]
    public void DateCreatedUtc_WhenDeserializedFromJsonUsingJsonNet_ShouldRetainDateTimeKindUtc()
    {
        // Arrange
        const string dateAsJson = "2022-12-01T12:30:45.1234567Z";
        var expectedDate = DateTime.ParseExact(dateAsJson, "O", CultureInfo.InvariantCulture).ToUniversalTime();
        var entityAsJson = /*lang=json,strict*/ @"
        {
            ""DateCreatedUtc"": ""{Date}"",
            ""DateModifiedUtc"": null
        }".Replace("{Date}", dateAsJson);

        // Act 
        var entity = JsonNet.JsonConvert.DeserializeObject<DummyEntity>(entityAsJson)!;

        // Assert
        Assert.Equal(expectedDate, entity.DateCreatedUtc);
        Assert.Equal(DateTimeKind.Utc, entity.DateCreatedUtc.Kind);
    }

    [Fact]
    public void DateCreatedUtc_WhenDeserializedFromJsonUsingTextJson_ShouldRetainDateTimeKindUtc()
    {
        // Arrange
        const string dateAsJson = "2022-12-01T12:30:45.1234567Z";
        var expectedDate = DateTime.ParseExact(dateAsJson, "O", CultureInfo.InvariantCulture).ToUniversalTime();
        var entityAsJson = /*lang=json,strict*/ @"
        {
            ""DateCreatedUtc"": ""{Date}"",
            ""DateModifiedUtc"": null
        }".Replace("{Date}", dateAsJson);

        // Act 
        var entity = TextJson.JsonSerializer.Deserialize<DummyEntity>(entityAsJson)!;

        // Assert
        Assert.Equal(expectedDate, entity.DateCreatedUtc);
        Assert.Equal(DateTimeKind.Utc, entity.DateCreatedUtc.Kind);
    }

    [Fact]
    public void DateModifiedUtc_WhenDeserializedFromJsonUsingJsonNet_RetainsDateTimeKindUtc()
    {
        // Arrange
        const string dateAsJson = "2022-12-01T12:30:45.1234567Z";
        var expectedDate = DateTime.ParseExact(dateAsJson, "O", CultureInfo.InvariantCulture).ToUniversalTime();
        var entityAsJson = /*lang=json,strict*/ @"
        {
            ""DateCreatedUtc"": ""2020-01-01T00:00:00.0000000Z"",
            ""DateModifiedUtc"": ""{Date}""
        }".Replace("{Date}", dateAsJson);

        // Act 
        var entity = JsonNet.JsonConvert.DeserializeObject<DummyEntity>(entityAsJson)!;

        // Assert 
        Assert.NotNull(entity.DateModifiedUtc);
        Assert.Equal(expectedDate, entity.DateModifiedUtc!.Value);
        Assert.Equal(DateTimeKind.Utc, entity.DateModifiedUtc.Value.Kind);
    }

    [Fact]
    public void DateModifiedUtc_WhenDeserializedFromJsonUsingTextJson_ShouldRetainDateTimeKindUtc()
    {
        // Arrange
        const string dateAsJson = "2022-12-01T12:30:45.1234567Z";
        var expectedDate = DateTime.ParseExact(dateAsJson, "O", CultureInfo.InvariantCulture).ToUniversalTime();
        var entityAsJson = /*lang=json,strict*/ @"
        {
            ""Id"": ""00000000-0000-0000-0000-000000000000"",
            ""DateCreatedUtc"": ""2020-01-01T00:00:00.0000000Z"",
            ""DateModifiedUtc"": ""{Date}""
        }".Replace("{Date}", dateAsJson);

        // Act 
        var entity = TextJson.JsonSerializer.Deserialize<DummyEntity>(entityAsJson)!;

        // Assert 
        Assert.NotNull(entity.DateModifiedUtc);
        Assert.Equal(expectedDate, entity.DateModifiedUtc!.Value);
        Assert.Equal(DateTimeKind.Utc, entity.DateModifiedUtc.Value.Kind);
    }

    [Fact]
    public void ToString_WhenIdIsNull_ShouldReturnNull()
    {
        // Arrange
        var entity = new DummyEntity();
        ((IUniqueEntity)entity).Id = null!;

        // Act
        var entityAsString = entity.ToString();

        // Assert
        Assert.Null(entityAsString);
    }

    [Fact]
    public void ToString_WhenNotOverriden_ShouldReturnId()
    {
        // Arrange
        var entity = new DummyEntity();
        var id = Guid.NewGuid();
        ((IUniqueEntity)entity).Id = id;
        var expected = id.ToString();

        // Act
        var entityAsString = entity.ToString();

        // Assert
        Assert.Equal(expected, entityAsString);
    }

    private sealed class DummyEntity : EntityBase
    {
    }
}

namespace Tests.Models.Common;

[TestFixture]
public sealed class EntityBaseTests
{
    // Methods
    [Test]
    public void EntityBase_ToString_ShouldReturnId()
    {
        // Arrange 
        var teacher = new Teacher
        {
            Id = Guid.NewGuid()
        };
        var expected = teacher.Id.ToString();

        // Act
        var result = teacher.ToString();

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    public void EntityBase_SetDateCreatedUtcWithNonUtcValue_ShouldSetDateTimeKindToUtc()
    {
        // Arrange
        var teacher = new Teacher();
        var dateCreated = DateTime.Now;
        var expected = dateCreated.ToUniversalTime();

        // Act
        teacher.DateCreatedUtc = dateCreated;
        var result = teacher.DateCreatedUtc;

        // Assert
        result.Should().Be(expected);
        result.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Test]
    public void EntityBase_SetDateModifiedUtcWithNonUtcValue_ShouldSetDateTimeKindToUtc()
    {
        // Arrange
        var teacher = new Teacher();
        var dateModified = DateTime.Now;
        var expected = dateModified.ToUniversalTime();

        // Act
        teacher.DateModifiedUtc = dateModified;
        var result = teacher.DateModifiedUtc.Value;

        // Assert
        result.Should().Be(expected);
        result.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Test]
    public void EntityBase_ToJsonUsingJsonNet_DateCreatedUtc_ShouldUseFormatO()
    {
        // Arrange
        const string dateAsString = "2022-12-01T12:30:45.1234567Z";
        var teacher = new Teacher
        {
            DateCreatedUtc = DateTime.ParseExact(dateAsString, "O", CultureInfo.InvariantCulture).ToUniversalTime()
        };
        var expected = $"\"{dateAsString}\"";

        // Act 
        var result = JsonNet.JsonConvert.SerializeObject(teacher.DateCreatedUtc);

        // Assert 
        result.Should().Be(expected);
    }

    [Test]
    public void EntityBase_ToJsonUsingTextJson_DateCreatedUtc_ShouldUseFormatO()
    {
        // Arrange
        const string dateAsString = "2022-12-01T12:30:45.1234567Z";
        var teacher = new Teacher
        {
            DateCreatedUtc = DateTime.ParseExact(dateAsString, "O", CultureInfo.InvariantCulture).ToUniversalTime()
        };
        var expected = $"\"{dateAsString}\"";

        // Act 
        var result = TextJson.JsonSerializer.Serialize(teacher.DateCreatedUtc);

        // Assert 
        result.Should().Be(expected);
    }

    [Test]
    public void EntityBase_ToJsonUsingJsonNet_DateModifiedUtc_ShouldUseFormatO()
    {
        // Arrange
        const string dateAsString = "2022-12-01T12:30:45.1234567Z";
        var teacher = new Teacher
        {
            DateModifiedUtc = DateTime.ParseExact(dateAsString, "O", CultureInfo.InvariantCulture).ToUniversalTime()
        };
        var expected = $"\"{dateAsString}\"";

        // Act 
        var result = JsonNet.JsonConvert.SerializeObject(teacher.DateModifiedUtc);

        // Assert 
        result.Should().Be(expected);
    }

    [Test]
    public void EntityBase_ToJsonUsingTextJson_DateModifiedUtc_ShouldUseFormatO()
    {
        // Arrange
        const string dateAsString = "2022-12-01T12:30:45.1234567Z";
        var teacher = new Teacher
        {
            DateModifiedUtc = DateTime.ParseExact(dateAsString, "O", CultureInfo.InvariantCulture).ToUniversalTime()
        };
        var expected = $"\"{dateAsString}\"";

        // Act 
        var result = TextJson.JsonSerializer.Serialize(teacher.DateModifiedUtc);

        // Assert 
        result.Should().Be(expected);
    }

    [Test]
    public void EntityBase_FromJsonUsingJsonNet_DateCreatedUtc_ShouldRetainDateTimeKindUtc()
    {
        // Arrange
        const string dateAsString = "2022-12-01T12:30:45.1234567Z";
        var expected = DateTime.ParseExact(dateAsString, "O", CultureInfo.InvariantCulture).ToUniversalTime();
        var teacherAsJson = /*lang=json,strict*/ @"
        {
            ""Name"": null,
            ""TeacherType"": 0,
            ""Courses"": [],
            ""Id"": ""00000000-0000-0000-0000-000000000000"",
            ""DateCreatedUtc"": ""{Date}"",
            ""DateModifiedUtc"": null
        }".Replace("{Date}", dateAsString);

        // Act 
        var teacher = JsonNet.JsonConvert.DeserializeObject<Teacher>(teacherAsJson);
        var result = teacher!.DateCreatedUtc;

        // Assert
        result.Should().Be(expected);
        result.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Test]
    public void EntityBase_FromJsonUsingTextJson_DateCreatedUtc_ShouldRetainDateTimeKindUtc()
    {
        // Arrange
        const string dateAsString = "2022-12-01T12:30:45.1234567Z";
        var expected = DateTime.ParseExact(dateAsString, "O", CultureInfo.InvariantCulture).ToUniversalTime();
        var teacherAsJson = /*lang=json,strict*/ @"
        {
            ""Name"": null,
            ""TeacherType"": 0,
            ""Courses"": [],
            ""Id"": ""00000000-0000-0000-0000-000000000000"",
            ""DateCreatedUtc"": ""{Date}"",
            ""DateModifiedUtc"": null
        }".Replace("{Date}", dateAsString);

        // Act 
        var teacher = TextJson.JsonSerializer.Deserialize<Teacher>(teacherAsJson);
        var result = teacher!.DateCreatedUtc;

        // Assert
        result.Should().Be(expected);
        result.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Test]
    public void EntityBase_FromJsonUsingJsonNet_DateModifiedUtcRetainsDateTimeKindUtc()
    {
        // Arrange
        const string dateAsString = "2022-12-01T12:30:45.1234567Z";
        var expected = DateTime.ParseExact(dateAsString, "O", CultureInfo.InvariantCulture).ToUniversalTime();
        var teacherAsJson = /*lang=json,strict*/ @"
        {
            ""Name"": null,
            ""TeacherType"": 0,
            ""Courses"": [],
            ""Id"": ""00000000-0000-0000-0000-000000000000"",
            ""DateCreatedUtc"": ""2020-01-01T00:00:00.0000000Z"",
            ""DateModifiedUtc"": ""{Date}""
        }".Replace("{Date}", dateAsString);

        // Act 
        var teacher = JsonNet.JsonConvert.DeserializeObject<Teacher>(teacherAsJson);
        var result = teacher!.DateModifiedUtc!.Value;

        // Assert
        result.Should().Be(expected);
        result.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Test]
    public void EntityBase_FromJsonUsingTextJson_DateModifiedUtc_ShouldRetainDateTimeKindUtc()
    {
        // Arrange
        const string dateAsString = "2022-12-01T12:30:45.1234567Z";
        var expected = DateTime.ParseExact(dateAsString, "O", CultureInfo.InvariantCulture).ToUniversalTime();
        var teacherAsJson = /*lang=json,strict*/ @"
        {
            ""Name"": null,
            ""TeacherType"": 0,
            ""Courses"": [],
            ""Id"": ""00000000-0000-0000-0000-000000000000"",
            ""DateCreatedUtc"": ""2020-01-01T00:00:00.0000000Z"",
            ""DateModifiedUtc"": ""{Date}""
        }".Replace("{Date}", dateAsString);

        // Act 
        var teacher = TextJson.JsonSerializer.Deserialize<Teacher>(teacherAsJson);
        var result = teacher!.DateModifiedUtc!.Value;

        // Assert
        result.Should().Be(expected);
        result.Kind.Should().Be(DateTimeKind.Utc);
    }
}
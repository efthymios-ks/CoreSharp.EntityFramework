namespace Tests.Models.Common;

[TestFixture]
public class EntityBaseTests
{
    //Methods
    [Test]
    public void EntityBase_ToString_ReturnsId()
    {
        //Arrange 
        var teacher = new Teacher
        {
            Id = Guid.NewGuid()
        };
        var expected = teacher.Id.ToString();

        //Act
        var result = teacher.ToString();

        //Assert
        result.Should().Be(expected);
    }

    [Test]
    public void EntityBase_SetDateCreatedUtcWithNonUtcValue_SetsDateTimeKindToUtc()
    {
        //Arrange
        var teacher = new Teacher();
        var dateCreated = DateTime.Now;
        var expected = dateCreated.ToUniversalTime();

        //Act
        teacher.DateCreatedUtc = dateCreated;
        var result = teacher.DateCreatedUtc;

        //Assert
        result.Should().Be(expected);
        result.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Test]
    public void EntityBase_SetDateModifiedUtcWithNonUtcValue_SetsDateTimeKindToUtc()
    {
        //Arrange
        var teacher = new Teacher();
        var dateModified = DateTime.Now;
        var expected = dateModified.ToUniversalTime();

        //Act
        teacher.DateModifiedUtc = dateModified;
        var result = teacher.DateModifiedUtc.Value;

        //Assert
        result.Should().Be(expected);
        result.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Test]
    public void EntityBase_ToJsonUsingJsonNet_DateCreatedUtcRetainsDateTimeKindUtc()
    {
        //Arrange
        var expected = DateTime.UtcNow;
        var teacher = new Teacher
        {
            DateCreatedUtc = expected
        };

        //Act
        var teacherAsJson = JsonNet.JsonConvert.SerializeObject(teacher);
        teacher = JsonNet.JsonConvert.DeserializeObject<Teacher>(teacherAsJson);
        var result = teacher!.DateCreatedUtc;

        //Assert
        result.Should().Be(expected);
        result.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Test]
    public void EntityBase_ToJsonUsingTextJson_DateCreatedUtcRetainsDateTimeKindUtc()
    {
        //Arrange
        var expected = DateTime.UtcNow;
        var teacher = new Teacher
        {
            DateCreatedUtc = expected
        };

        //Act
        var teacherAsJson = TextJson.JsonSerializer.Serialize(teacher);
        teacher = TextJson.JsonSerializer.Deserialize<Teacher>(teacherAsJson);
        var result = teacher!.DateCreatedUtc;

        //Assert
        result.Should().Be(expected);
        result.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Test]
    public void EntityBase_ToJsonUsingJsonNet_DateModifiedUtcRetainsDateTimeKindUtc()
    {
        //Arrange
        var expected = DateTime.UtcNow;
        var teacher = new Teacher
        {
            DateModifiedUtc = expected
        };

        //Act
        var teacherAsJson = JsonNet.JsonConvert.SerializeObject(teacher);
        teacher = JsonNet.JsonConvert.DeserializeObject<Teacher>(teacherAsJson);
        var result = teacher!.DateModifiedUtc!.Value;

        //Assert
        result.Should().Be(expected);
        result!.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Test]
    public void EntityBase_ToJsonUsingTextJson_DateModifiedUtcRetainsDateTimeKindUtc()
    {
        //Arrange
        var expected = DateTime.UtcNow;
        var teacher = new Teacher
        {
            DateModifiedUtc = expected
        };

        //Act
        var teacherAsJson = TextJson.JsonSerializer.Serialize(teacher);
        teacher = TextJson.JsonSerializer.Deserialize<Teacher>(teacherAsJson);
        var result = teacher!.DateModifiedUtc!.Value;

        //Assert
        result.Should().Be(expected);
        result.Kind.Should().Be(DateTimeKind.Utc);
    }
}
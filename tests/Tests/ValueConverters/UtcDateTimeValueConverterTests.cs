using CoreSharp.EntityFramework.ValueComparers;

namespace Tests.ValueConverters;

[TestFixture]
public sealed class UtcDateTimeValueConverterTests
{
    [TestCase]
    public void Constructor_WhenCalled_ShouldNotThrowException()
    {
        // Assert
        Action action = () => _ = new UtcDateTimeValueComparer();

        // Arrange
        action.Should().NotThrow();
    }

    [TestCase(null, null, true)]
    [TestCase(null, "2024-04-28T12:00:00", false)]
    [TestCase("2024-04-28T12:00:00", null, false)]
    [TestCase("2024-04-28T12:00:00", "2024-04-28T12:00:00", true)]
    [TestCase("2024-04-28T12:00:00", "2024-04-29T12:00:00", false)]
    [TestCase("2024-04-28T12:00:00Z", "2024-04-28T12:00:00Z", true)]
    [TestCase("2024-04-28T12:00:00Z", "2024-04-29T12:00:00Z", false)]
    public void Equals_WhenDatesHaveSameKind_ShouldReturnTrueWhenValuesMatch(string dateTime1AsString, string dateTime2AsString, bool expectedReslt)
    {
        // Arrange
        var comparer = new UtcDateTimeValueComparer();
        var dateTime1 = ParseDateTime(dateTime1AsString);
        var dateTime2 = ParseDateTime(dateTime2AsString);

        // Act
        var result = comparer.Equals(dateTime1, dateTime2);

        // Assert
        result.Should().Be(expectedReslt);
    }

    [TestCase]
    public void Equals_WhenDatesHaveHaveDifferentKindAndTheirUtcValuesMatch_ShouldReturnTrue()
    {
        // Arrange
        var comparer = new UtcDateTimeValueComparer();
        var dateTime1 = DateTime.Now;
        var dateTime2 = dateTime1.ToUniversalTime();

        // Act
        var result = comparer.Equals(dateTime1, dateTime2);

        // Assert
        result.Should().BeTrue();
    }

    [TestCase]
    public void Equals_WhenDatesHaveHaveDifferentKindAndTheirUtcValuesDiffer_ShouldReturnFalse()
    {
        // Arrange
        var comparer = new UtcDateTimeValueComparer();
        var dateTime1 = DateTime.Now;
        var dateTime2 = dateTime1.ToUniversalTime().AddHours(1);

        // Act
        var result = comparer.Equals(dateTime1, dateTime2);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void GetHashCode_WhenDateTimeIsNull_ShouldReturnZero()
    {
        // Arrange
        var comparer = new UtcDateTimeValueComparer();
        DateTime? dateTime = null;

        // Act
        var hashCode = comparer.GetHashCode(dateTime);

        // Assert
        hashCode.Should().Be(0);
    }

    [Test]
    public void GetHashCode_WhenDateTimeIsNotNull_ShouldReturnHashCode()
    {
        // Arrange
        var comparer = new UtcDateTimeValueComparer();
        var dateTime = DateTime.UtcNow;

        // Act
        var hashCode = comparer.GetHashCode(dateTime);

        // Assert
        hashCode.Should().NotBe(0);
    }

    [Test]
    public void Snapshot_WhenCalled_ShouldReturnSameValue()
    {
        var comparer = new UtcDateTimeValueComparer();
        var dateTime = DateTime.UtcNow;

        // Act
        var result = comparer.Snapshot(dateTime);

        // Assert
        result.Should().Be(dateTime);
    }

    [Test]
    public void Instance_WhenCalled_ShouldNotBeNull()
    {
        // Act
        var instance1 = UtcDateTimeValueComparer.Instance;

        // Assert
        instance1.Should().NotBeNull();
    }

    [Test]
    public void Instance_WhenCalledMultipleTimes_ShouldReturnSameInstance()
    {
        // Act
        var instance1 = UtcDateTimeValueComparer.Instance;
        var instance2 = UtcDateTimeValueComparer.Instance;

        // Assert
        instance1.Should().BeSameAs(instance2);
    }

    private static DateTime? ParseDateTime(string dateTimeAsString)
    {
        if (string.IsNullOrWhiteSpace(dateTimeAsString))
        {
            return null;
        }

        if (DateTime.TryParseExact(dateTimeAsString,
            "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal,
            out var dateTime))
        {
            return dateTime.ToUniversalTime();
        }

        if (DateTime.TryParseExact(dateTimeAsString,
            "yyyy-MM-ddTHH:mm:ss",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeLocal,
            out dateTime))
        {
            return dateTime;
        }

        return null;
    }
}

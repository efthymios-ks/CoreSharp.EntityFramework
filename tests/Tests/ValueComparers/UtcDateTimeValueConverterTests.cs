using CoreSharp.EntityFramework.ValueConverters;

namespace Tests.ValueComparers;

[TestFixture]
public sealed class UtcDateTimeValueConverterTests
{
    [Test]
    public void ConvertToProvider_WhenDateTimeIsNull_ShouldReturnNull()
    {
        // Arrange
        var converter = new UtcDateTimeValueConverter();
        DateTime? dateTime = null;

        // Act
        var result = converter.ConvertToProvider(dateTime);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void ConvertToProvider_WhenDateTimeIsNotNull_ShouldReturnUniversalDate()
    {
        // Arrange
        var converter = new UtcDateTimeValueConverter();
        var dateTime = DateTime.Now;
        var expectedDateTime = dateTime.ToUniversalTime();

        // Act
        var result = converter.ConvertToProvider(dateTime);

        // Assert
        result.Should().BeEquivalentTo(expectedDateTime);
    }

    [Test]
    public void ConvertFromProvider_WhenDateTimeIsNull_ShouldReturnNull()
    {
        // Arrange
        var converter = new UtcDateTimeValueConverter();
        DateTime? dateTime = null;

        // Act
        var result = converter.ConvertFromProvider(dateTime);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void ConvertFromProvider_WhenDateTimeIsNotNull_ShouldReturnUniversalDate()
    {
        // Arrange
        var converter = new UtcDateTimeValueConverter();
        var dateTime = DateTime.Now;
        var expectedDateTime = dateTime.ToUniversalTime();

        // Act
        var result = converter.ConvertFromProvider(dateTime);

        // Assert
        result.Should().BeEquivalentTo(expectedDateTime);
    }

    [Test]
    public void Instance_WhenCalled_ShouldNotBeNull()
    {
        // Act
        var instance1 = UtcDateTimeValueConverter.Instance;

        // Assert
        instance1.Should().NotBeNull();
    }

    [Test]
    public void Instance_WhenCalledMultipleTimes_ShouldReturnSameInstance()
    {
        // Act
        var instance1 = UtcDateTimeValueConverter.Instance;
        var instance2 = UtcDateTimeValueConverter.Instance;

        // Assert
        instance1.Should().BeSameAs(instance2);
    }
}

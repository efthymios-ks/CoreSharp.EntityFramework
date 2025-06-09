using CoreSharp.EntityFramework.Bulkoperations.Extensions;

namespace CoreSharp.EntityFramework.Tests.BulkOperations.Extensions;

public sealed class TypeExtensionsTests
{
    [Fact]
    public void GetSqlDbType_WhenTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        Type? type = null;

        // Act
        void Action()
            => type!.GetSqlDbType();

        // Assert
        Assert.Throws<ArgumentNullException>(Action);
    }

    [Theory]
    [InlineData(typeof(string), "nvarchar(max)")]
    [InlineData(typeof(char), "nchar(1)")]
    [InlineData(typeof(byte[]), "varbinary(max)")]
    [InlineData(typeof(int), "int")]
    [InlineData(typeof(uint), "bigint")]
    [InlineData(typeof(long), "bigint")]
    [InlineData(typeof(ulong), "decimal(20,0)")]
    [InlineData(typeof(short), "smallint")]
    [InlineData(typeof(ushort), "int")]
    [InlineData(typeof(byte), "tinyint")]
    [InlineData(typeof(sbyte), "smallint")]
    [InlineData(typeof(decimal), "decimal(18,2)")]
    [InlineData(typeof(double), "float")]
    [InlineData(typeof(float), "real")]
    [InlineData(typeof(bool), "bit")]
    [InlineData(typeof(DateTime), "datetime2")]
    [InlineData(typeof(DateOnly), "date")]
    [InlineData(typeof(DateTimeOffset), "datetimeoffset")]
    [InlineData(typeof(TimeSpan), "time")]
    [InlineData(typeof(TimeOnly), "time")]
    [InlineData(typeof(Guid), "uniqueidentifier")]
    [InlineData(typeof(int?), "int")]
    [InlineData(typeof(DummyLongEnum), "bigint")]
    public void GetSqlDbType_WhenCalled_ShouldReturnCorrectSqlDbType(Type inputType, string expectedSqlDbType)
    {
        // Act
        var sqlDbType = inputType.GetSqlDbType();

        // Assert
        Assert.Equal(expectedSqlDbType, sqlDbType);
    }

    [Fact]
    public void GetSqlDbType_WhenTypeIsUnsupported_ShouldThrowNotSupportedException()
    {
        // Arrange
        var unsupportedType = typeof(TypeExtensionsTests);

        // Act
        void Action()
            => unsupportedType.GetSqlDbType();

        // Assert
        var exception = Assert.Throws<NotSupportedException>(Action);
        Assert.Contains(unsupportedType.FullName!, exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    private enum DummyLongEnum : long;
}

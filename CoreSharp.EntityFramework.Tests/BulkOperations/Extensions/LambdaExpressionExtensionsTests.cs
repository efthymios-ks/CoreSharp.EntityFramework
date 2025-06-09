using CoreSharp.EntityFramework.Bulkoperations.Extensions;
using System.Linq.Expressions;

namespace CoreSharp.EntityFramework.Tests.BulkOperations.Extensions;

public sealed class LambdaExpressionExtensionsTests
{
    [Fact]
    public void GetProperty_WhenExpressionIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        LambdaExpression? expression = null;

        // Act
        void Action()
            => expression!.GetProperty();

        // Assert
        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
    public void GetProperty_WhenExpressionIsMemberAndNotProperty_ShouldThrowInvalidOperationException()
    {
        // Arrange
        Expression<Func<TestClass, int>> expression = x => x.Field;

        // Act
        void Action()
            => expression.GetProperty();

        // Assert
        Assert.Throws<InvalidOperationException>(Action);
    }

    [Fact]
    public void GetProperty_WhenExpressionIsMemberAndProperty_ShouldReturnPropertyInfo()
    {
        // Arrange
        Expression<Func<TestClass, int>> expression = x => x.Property;

        // Act
        var propertyInfo = expression.GetProperty();

        // Assert
        Assert.NotNull(propertyInfo);
        Assert.Equal(nameof(TestClass.Property), propertyInfo.Name);
    }

    [Fact]
    public void GetProperty_WhenExpressionIsUnaryAndNotMember_ShouldThrowInvalidOperationException()
    {
        // Arrange
        Expression<Func<TestClass, object>> expression = x => x.GetType();

        // Act
        void Action()

            => expression.GetProperty();
        // Assert
        Assert.Throws<InvalidOperationException>(Action);
    }

    [Fact]
    public void GetProperty_WhenExpressionIsUnaryAndMemberAndNotProperty_ShouldThrowInvalidOperationException()
    {
        // Arrange
        Expression<Func<TestClass, object>> expression = x => x.Field;

        // Act
        void Action()
            => expression.GetProperty();

        // Assert
        Assert.Throws<InvalidOperationException>(Action);
    }

    [Fact]
    public void GetProperty_WhenExpressionIsUnaryAndMemberAndProperty_ShouldReturnPropertyInfo()
    {
        // Arrange
        Expression<Func<TestClass, object>> expression = x => x.Property;

        // Act
        var propertyInfo = expression.GetProperty();

        // Assert
        Assert.NotNull(propertyInfo);
        Assert.Equal(nameof(TestClass.Property), propertyInfo.Name);
    }

    [Fact]
    public void GetProperty_WhenExpressionIsInvalid_ShouldThrowInvalidOperationException()
    {
        // Arrange
        Expression<Func<TestClass, int>> expression = x => 1 + 1;

        // Act
        void Action()
            => expression.GetProperty();

        // Assert
        Assert.Throws<InvalidOperationException>(Action);
    }

    private sealed class TestClass
    {
        public int Field = 0;
        public int Property { get; set; }
    }
}

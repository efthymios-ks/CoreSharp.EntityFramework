namespace CoreSharp.EntityFramework.Extensions.Tests;

[TestFixture]
public sealed class IQueryableExtensionsTests
{
    [Test]
    public async Task GetPageAsync_WhenCalled_ShouldReturnCorrectPageNumberAndSize()
    {
        // Arrange 
        const int pageNumber = 2;
        const int pageSize = 5;
        var query = Enumerable.Range(1, 20).AsQueryable();

        // Act
        var result = await query.GetPageAsync(pageNumber, pageSize);

        // Assert
        result.Should().NotBeNull();
        result.PageNumber.Should().Be(pageNumber);
        result.PageSize.Should().Be(pageSize);
    }

    [Test]
    public async Task GetPageAsync_WhenCalled_ShouldReturnCorrectTotalItems()
    {
        // Arrange 
        const int pageNumber = 2;
        const int pageSize = 5;
        var query = Enumerable.Range(1, 20).AsQueryable();

        // Act
        var result = await query.GetPageAsync(pageNumber, pageSize);

        // Assert
        result.Should().NotBeNull();
        result.TotalItems.Should().Be(20);
    }

    [Test]
    public async Task GetPageAsync_WhenCalled_ShouldReturnCorrectTotalPages()
    {
        // Arrange 
        const int pageNumber = 2;
        const int pageSize = 5;
        var query = Enumerable.Range(1, 20).AsQueryable();

        // Act
        var result = await query.GetPageAsync(pageNumber, pageSize);

        // Assert
        result.Should().NotBeNull();
        result.TotalPages.Should().Be(4);
    }

    [Test]
    public async Task GetPageAsync_WhenCalled_ShouldReturnCorrectItemsInPage()
    {
        // Arrange
        const int pageNumber = 2;
        const int pageSize = 5;
        var query = Enumerable.Range(1, 20).AsQueryable();

        // Act
        var result = await query.GetPageAsync(pageNumber, pageSize);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(pageSize);
        result.Items.Should().ContainInOrder(11, 12, 13, 14, 15);
    }

    [Test]
    public async Task GetPageAsync_WhenPageNumberIsNegative_ShouldThrowException()
    {
        // Arrange & Act
        const int pageNumber = -1;
        const int pageSize = 5;
        var query = Enumerable.Range(1, 20).AsQueryable();

        // Act 
        Func<Task> action = () => query.GetPageAsync(pageNumber, pageSize);

        // Assert
        await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task GetPageAsync_WhenPageSizeIsZero_ShouldThrowException()
    {
        // Arrange & Act
        const int pageNumber = 2;
        const int pageSize = 0;
        var query = Enumerable.Range(1, 20).AsQueryable();

        // Act 
        Func<Task> action = () => query.GetPageAsync(pageNumber, pageSize);

        // Assert
        await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task GetPageAsync_WhenPageSizeIsNegative_ShouldThrowException()
    {
        // Arrange & Act
        const int pageNumber = 2;
        const int pageSize = -5;
        var query = Enumerable.Range(1, 20).AsQueryable();

        // Act 
        Func<Task> action = () => query.GetPageAsync(pageNumber, pageSize);

        // Assert
        await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }
}

using CoreSharp.EntityFramework.Extensions;

namespace CoreSharp.EntityFramework.Tests.Extensions;

public sealed class IQueryableExtensionsTests
{
    [Fact]
    public async Task GetPageAsync_WhenCalled_ShouldReturnCorrectPageNumberAndSize()
    {
        // Arrange 
        const int pageNumber = 2;
        const int pageSize = 5;
        var query = Enumerable.Range(1, 20).AsQueryable();

        // Act
        var result = await query.GetPageAsync(pageNumber, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(pageNumber, result.PageNumber);
        Assert.Equal(pageSize, result.PageSize);
    }

    [Fact]
    public async Task GetPageAsync_WhenCalled_ShouldReturnCorrectTotalItems()
    {
        // Arrange 
        const int pageNumber = 2;
        const int pageSize = 5;
        var query = Enumerable.Range(1, 20).AsQueryable();

        // Act
        var result = await query.GetPageAsync(pageNumber, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(20, result.TotalItems);
    }

    [Fact]
    public async Task GetPageAsync_WhenCalled_ShouldReturnCorrectTotalPages()
    {
        // Arrange 
        const int pageNumber = 2;
        const int pageSize = 5;
        var query = Enumerable.Range(1, 20).AsQueryable();

        // Act
        var result = await query.GetPageAsync(pageNumber, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.TotalPages);
    }

    [Fact]
    public async Task GetPageAsync_WhenCalled_ShouldReturnCorrectItemsInPage()
    {
        // Arrange
        const int pageNumber = 2;
        const int pageSize = 5;
        var query = Enumerable.Range(1, 20).AsQueryable();
        var expectedItems = new[] { 11, 12, 13, 14, 15 };

        // Act
        var result = await query.GetPageAsync(pageNumber, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(pageSize, result.Items.Count());
        Assert.Equal(expectedItems, result.Items);
    }

    [Fact]
    public async Task GetPageAsync_WhenPageNumberIsNegative_ShouldThrowException()
    {
        // Arrange & Act
        const int pageNumber = -1;
        const int pageSize = 5;
        var query = Enumerable.Range(1, 20).AsQueryable();

        // Act 
        Task Action()
            => query.GetPageAsync(pageNumber, pageSize);

        // Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(Action);
    }

    [Fact]
    public async Task GetPageAsync_WhenPageSizeIsZero_ShouldThrowException()
    {
        // Arrange & Act
        const int pageNumber = 2;
        const int pageSize = 0;
        var query = Enumerable.Range(1, 20).AsQueryable();

        // Act 
        Task Action()
            => query.GetPageAsync(pageNumber, pageSize);

        // Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(Action);
    }

    [Fact]
    public async Task GetPageAsync_WhenPageSizeIsNegative_ShouldThrowException()
    {
        // Arrange & Act
        const int pageNumber = 2;
        const int pageSize = -5;
        var query = Enumerable.Range(1, 20).AsQueryable();

        // Act 
        Task Action()
            => query.GetPageAsync(pageNumber, pageSize);

        // Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(Action);
    }
}

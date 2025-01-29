namespace CoreSharp.EntityFramework.Tests.Delegates;

public sealed class QueryDelegateTests
{
    [Fact]
    public void FilterQuery_WheQueryFilters_ShouldReturnedFilteredItems()
    {
        // Arrange
        var query = new[] { 1, 2, 3, 4, 5 }.AsQueryable();
        static IQueryable<int> Filter(IQueryable<int> query) => query.Where(number => number % 2 == 0);

        // Act
        var filteredQuery = Filter(query);

        // Assert
        Assert.NotNull(filteredQuery);
        Assert.IsAssignableFrom<IQueryable<int>>(filteredQuery);
        Assert.Equal([2, 4], filteredQuery);
    }

    [Fact]
    public void SortQuery_WhenQuerySorts_ShouldReturnSortedItems()
    {
        // Arrange
        var query = new[] { 3, 1, 2 }.AsQueryable();
        static IQueryable<int> Sorter(IQueryable<int> query) => query.OrderBy(number => number);

        // Act
        var sortedQuery = Sorter(query);

        // Assert
        Assert.NotNull(sortedQuery);
        Assert.IsAssignableFrom<IQueryable<int>>(sortedQuery);
        Assert.Equal([1, 2, 3], sortedQuery);
    }

    [Fact]
    public void IdentityQuery_WhenQueryIsEmpty_ShouldReturnUnchangedItems()
    {
        // Arrange
        var query = new[] { 1, 2, 3 }.AsQueryable();
        static IQueryable<int> NoOp(IQueryable<int> query) => query;

        // Act
        var result = NoOp(query);

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IQueryable<int>>(result);
        Assert.Equal([1, 2, 3], result);
    }

    [Fact]
    public void FilterQuery_WhenQueryMatchesNothing_ShouldReturnNoItems()
    {
        // Arrange
        var query = new[] { 1, 3, 5 }.AsQueryable();
        static IQueryable<int> Filter(IQueryable<int> query) => query.Where(x => x % 2 == 0);

        // Act
        var result = Filter(query);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void FilterQuery_WhenSourceIsEmpty_ShouldReturnNoData()
    {
        // Arrange
        var query = Enumerable.Empty<int>().AsQueryable();
        static IQueryable<int> Filter(IQueryable<int> query) => query.Where(x => x % 2 == 0);

        // Act
        var result = Filter(query);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}

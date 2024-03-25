using CoreSharp.EntityFramework.Delegates;
using System.Diagnostics.CodeAnalysis;

namespace Tests.Delegates;

[TestFixture]
[SuppressMessage("Style", "IDE0039:Use local function", Justification = "<Pending>")]
public sealed class QueryDelegateTests
{
    [Test]
    public void FilterQuery_WheQueryFilters_ShouldReturnedFilteredItems()
    {
        // Arrange
        var query = new[] { 1, 2, 3, 4, 5 }.AsQueryable();
        Query<int> filter = query => query.Where(number => number % 2 == 0);

        // Act
        var filteredQuery = filter(query);

        // Assert
        filteredQuery.Should().NotBeNull();
        filteredQuery.Should().BeAssignableTo<IQueryable<int>>();
        filteredQuery.Should().ContainInOrder(2, 4);
    }

    [Test]
    public void SortQuery_WhenQuerySorts_ShouldReturnSortedItems()
    {
        // Arrange
        var query = new[] { 3, 1, 2 }.AsQueryable();
        Query<int> sorter = query => query.OrderBy(number => number);

        // Act
        var sortedQuery = sorter(query);

        // Assert
        sortedQuery.Should().NotBeNull();
        sortedQuery.Should().BeAssignableTo<IQueryable<int>>();
        sortedQuery.Should().ContainInOrder(1, 2, 3);
    }

    [Test]
    public void IdentityQuery_WhenQueryIsEmpty_ShouldReturnUnchangedItems()
    {
        // Arrange
        var query = new[] { 1, 2, 3 }.AsQueryable();
        Query<int> noOp = query => query;

        // Act
        var result = noOp(query);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IQueryable<int>>();
        result.Should().ContainInOrder(1, 2, 3);
    }

    [Test]
    public void FilterQuery_WhenQueryMatchesNothing_ShouldReturnNoItems()
    {
        // Arrange
        var query = new[] { 1, 3, 5 }.AsQueryable();
        Query<int> filter = query => query.Where(x => x % 2 == 0);

        // Act
        var result = filter(query);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Test]
    public void FilterQuery_WhenSourceIsEmpty_ShouldReturnNoData()
    {
        // Arrange
        var query = Enumerable.Empty<int>().AsQueryable();
        Query<int> filter = query => query.Where(x => x % 2 == 0);

        // Act
        var result = filter(query);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}

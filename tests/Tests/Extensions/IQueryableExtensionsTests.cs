using CoreSharp.EntityFramework.Extensions;
using System.Collections;
using System.Linq.Expressions;

namespace Tests.Extensions;

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

    public async Task GetPageAsync_WhenQueryableIsIAsyncEnumerable_ShouldThrowException()
    {
        // Arrange
        const int pageNumber = 2;
        const int pageSize = 5;
        var source = Enumerable.Range(1, 20).AsQueryable();
        var query = new AsyncQueryable<int>(source);

        // Act 
        var result = await query.GetPageAsync(pageNumber, pageSize);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(pageSize);
        result.Items.Should().ContainInOrder(11, 12, 13, 14, 15);
    }

    private sealed class AsyncQueryable<TEntity> : IQueryable<TEntity>, IAsyncEnumerable<TEntity>
    {
        private readonly IQueryable<TEntity> _queryable;

        internal AsyncQueryable(IQueryable<TEntity> source)
            => _queryable = source;

        public Type ElementType
            => _queryable.ElementType;

        public Expression Expression
            => _queryable.Expression;

        public IQueryProvider Provider
            => _queryable.Provider;

        public IEnumerator<TEntity> GetEnumerator()
            => _queryable.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public IAsyncEnumerator<TEntity> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new AsyncEnumerator<TEntity>(_queryable.GetEnumerator());
    }

    private sealed class AsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _enumerator;

        public AsyncEnumerator(IEnumerator<T> enumerator)
            => _enumerator = enumerator;

        public T Current
            => _enumerator.Current;

        public ValueTask<bool> MoveNextAsync()
        {
            var hasMore = _enumerator.MoveNext();
            return ValueTask.FromResult(hasMore);
        }

        public ValueTask DisposeAsync()
        {
            _enumerator.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}

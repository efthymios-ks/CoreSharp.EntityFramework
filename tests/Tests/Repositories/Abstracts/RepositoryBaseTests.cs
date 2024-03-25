using CoreSharp.EntityFramework.Repositories.Abstracts;
using Microsoft.EntityFrameworkCore;
using Tests.Internal.Database.Models;

namespace Tests.Repositories.Abstracts;

[TestFixture]
public sealed class RepositoryBaseTests : DummyDbContextTestsBase
{
    [Test]
    public void Constructor_WhenDbContextIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        Action action = () => _ = new DummyRepository(dbContext: null);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    [Test]
    public async Task GetAsync_WithKey_WhenKeyIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var repository = new DummyRepository(DbContext);

        // Act
        Func<Task> func = () => _ = repository.GetAsync(key: null);

        // Assert
        await func.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test]
    public async Task GetAsync_WithKey_WhenNavigationIsNull_ShouldNotThrowArgumentNullException()
    {
        // Arrange
        var repository = new DummyRepository(DbContext);

        // Act
        Func<Task> func = () => _ = repository.GetAsync(Guid.NewGuid(), navigation: null);

        // Assert
        await func.Should().NotThrowAsync();
    }

    [Test]
    public async Task GetAsync_WithKey_WhenNavigationIsNotSet_ShouldReturnUnfilteredEntity()
    {
        // Arrange
        var existingDummyId = (await PreloadDummiesAsync(1))[0].Id;
        var repository = new DummyRepository(DbContext);

        // Act
        var dummy = await repository.GetAsync(existingDummyId, query => query);

        // Assert
        dummy.Should().NotBeNull();
    }

    [Test]
    public async Task GetAsync_WithKey_WhenNavigationIsSet_ShouldReturnFilteredEntity()
    {
        // Arrange
        var existingDummyId = (await PreloadDummiesAsync(1))[0].Id;
        var repository = new DummyRepository(DbContext);

        // Act
        var dummy = await repository.GetAsync(existingDummyId, query => query.Where(d => d.DateCreatedUtc == DateTime.MinValue));

        // Assert
        dummy.Should().BeNull();
    }

    [Test]
    public async Task GetAsync_WithKey_WhenCancellationTokenIsSet_ShouldThrowTaskCancelledException()
    {
        // Arrange
        var repository = new DummyRepository(DbContext);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> func = () => _ = repository.GetAsync(Guid.NewGuid(), cancellationToken: cancellationTokenSource.Token);

        // Assert
        await func.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task GetAsync_WithKey_WhenKeyNotFound_ShouldReturnNull()
    {
        // Arrange
        await PreloadDummiesAsync(1);
        var repository = new DummyRepository(DbContext);

        // Act
        var dummy = await repository.GetAsync(Guid.NewGuid());

        // Assert
        dummy.Should().BeNull();
    }

    [Test]
    public async Task GetAsync_WithKey_WhenKeyFound_ShouldReturnEntity()
    {
        // Arrange
        var existingDummy = (await PreloadDummiesAsync(1))[0];
        var repository = new DummyRepository(DbContext);

        // Act
        var readDummy = await repository.GetAsync(existingDummy.Id);

        // Assert
        readDummy.Should().BeEquivalentTo(existingDummy);
    }

    [Test]
    public async Task GetAsync_All_WhenNavigationIsNull_ShouldNotThrowArgumentNullException()
    {
        // Arrange
        var repository = new DummyRepository(DbContext);

        // Act
        Func<Task> func = () => _ = repository.GetAsync(navigation: null);

        // Assert
        await func.Should().NotThrowAsync();
    }

    [Test]
    public async Task GetAsync_All_WhenNavigationIsNotSet_ShouldReturnUnfilteredEntities()
    {
        // Arrange
        var existingDummies = await PreloadDummiesAsync(2);
        var repository = new DummyRepository(DbContext);

        // Act
        var readDummies = await repository.GetAsync(navigation: query => query);

        // Assert
        readDummies.Should().BeEquivalentTo(existingDummies);
    }

    [Test]
    public async Task GetAsync_All_WhenNavigationIsSet_ShouldReturnFilteredEntity()
    {
        // Arrange
        await PreloadDummiesAsync(1);
        var repository = new DummyRepository(DbContext);

        // Act
        var dummies = await repository.GetAsync(query => query.Where(d => d.DateCreatedUtc == DateTime.MinValue));

        // Assert
        dummies.Should().BeEmpty();
    }

    [Test]
    public async Task GetAsync_All_WhenCancellationTokenIsSet_ShouldThrowTaskCancelledException()
    {
        // Arrange
        var repository = new DummyRepository(DbContext);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> func = () => _ = repository.GetAsync(cancellationToken: cancellationTokenSource.Token);

        // Assert
        await func.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task AddAsync_WhenEntityIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var repository = new DummyRepository(DbContext);

        // Act
        Func<Task> func = () => _ = repository.AddAsync(entity: null);

        // Assert
        await func.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Test(Description = "Should not throw, because token is not used internally.")]
    public async Task AddAsync_WhenCancellationTokenIsSet_ShouldNotThrowTaskCancelledException()
    {
        // Arrange
        var dummyToAdd = GenerateDummy();
        var repository = new DummyRepository(DbContext);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> func = () => _ = repository.AddAsync(dummyToAdd, cancellationToken: cancellationTokenSource.Token);

        // Assert
        await func.Should().NotThrowAsync<TaskCanceledException>();
    }

    [Test]
    public async Task AddAsync_WhenCalled_ShouldAddEntityAndReturnIt()
    {
        // Arrange
        var dummyToAdd = GenerateDummy();
        var repository = new DummyRepository(DbContext);

        // Act
        var dummyIdBeforeAdding = dummyToAdd.Id;
        var addedDummy = await repository.AddAsync(dummyToAdd);
        var dummyIdAfterAdding = addedDummy.Id;

        // Assert
        dummyIdBeforeAdding.Should().Be(Guid.Empty);
        dummyIdAfterAdding.Should().NotBe(dummyIdBeforeAdding);
        addedDummy.Should().BeEquivalentTo(dummyToAdd);
    }

    private sealed class DummyRepository : RepositoryBase<DummyEntity>
    {
        public DummyRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}

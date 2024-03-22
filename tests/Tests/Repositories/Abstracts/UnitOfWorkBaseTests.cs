using CoreSharp.EntityFramework.Repositories.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace Tests.Repositories.Abstracts;

[TestFixture]
public sealed class UnitOfWorkBaseTests : AppDbContextTestsBase
{
    [Test]
    public void Constructor_WhenDbContextIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange 
        DbContext dbContext = null;

        // Act
        Action action = () => _ = new TestUnitOfWork(dbContext);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    [Test]
    public async Task CommitAsync_WhenCalled_ShouldSaveChanges()
    {
        // Arrange   
        var teacherToAdd = new Teacher
        {
            Name = Guid.NewGuid().ToString()
        };
        var unitOfWork = new TestUnitOfWork(AppDbContext);

        // Act
        await AppDbContext.Teachers.AddAsync(teacherToAdd);
        await unitOfWork.CommitAsync();
        var teachers = await AppDbContext.Teachers.ToArrayAsync();

        // Assert  
        teachers.Should().BeEquivalentTo(new[] { teacherToAdd });
    }

    [Test]
    public async Task CommitAsync_WhenCancellationTokenIsSet_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var unitOfWork = new TestUnitOfWork(AppDbContext);
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        await AppDbContext.Teachers.AddAsync(new());
        Func<Task> func = () => unitOfWork.CommitAsync(cancellationTokenSource.Token);

        // Assert  
        await func.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task RollbackAsync_WhenCalled_ShouldRollbackPendingChanges()
    {
        // Arrange   
        var teacherToAdd = new Teacher();
        var unitOfWork = new TestUnitOfWork(AppDbContext);

        // Act
        await AppDbContext.Teachers.AddAsync(teacherToAdd);
        await unitOfWork.RollbackAsync();
        var teachers = await AppDbContext.Teachers.ToArrayAsync();

        // Assert 
        teachers.Should().BeEmpty();
    }

    [Test]
    public async Task RollbackAsync_WhenCancellationTokenIsSetAndEntityDeleted_ShouldThrowTaskCancelledException()
    {
        // Arrange 
        var unitOfWork = new TestUnitOfWork(AppDbContext);
        var existingTeachers = await InsertTeachersAsync(1);
        AppDbContext.Teachers.RemoveRange(existingTeachers);
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act 
        Func<Task> func = () => unitOfWork.RollbackAsync(cancellationTokenSource.Token);

        // Assert  
        await func.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Test]
    public async Task DisposeAsync_ShouldCallDisposeAsyncOnContext()
    {
        // Arrange 
        var unitOfWork = new TestUnitOfWork(AppDbContext);
        await unitOfWork.DisposeAsync();

        // Act
        Func<Task> func = () => unitOfWork.CommitAsync();

        // Assert
        await func.Should().ThrowExactlyAsync<ObjectDisposedException>();
    }

    private class TestUnitOfWork : UnitOfWorkBase
    {
        public TestUnitOfWork(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
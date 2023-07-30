using Domain.Database;
using Microsoft.EntityFrameworkCore;

namespace Tests.Internal.Abstracts;

public abstract class AppDbContextTestsBase
{
    // Properties
    protected AppDbContext AppDbContext { get; set; }

    // Methods
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var databaseName = $"{nameof(Domain.Database.AppDbContext)}_{DateTime.Now.ToFileTimeUtc()}";
        var options = new DbContextOptionsBuilder<AppDbContext>()
                        .UseInMemoryDatabase(databaseName: databaseName)
                        .EnableDetailedErrors()
                        .EnableSensitiveDataLogging()
                        .Options;
        AppDbContext = new AppDbContext(options);
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDownAsync()
        => await AppDbContext.DisposeAsync();

    [SetUp]
    public async Task SetUpAsync()
        => await AppDbContext.Database.EnsureCreatedAsync();

    [TearDown]
    public async Task TearDownAsync()
        => await AppDbContext.Database.EnsureDeletedAsync();

    protected async Task<Teacher[]> PopulateTeachersAsync(int count)
    {
        var teachers = CreateTeachers(count);
        await AppDbContext.AddRangeAsync(teachers);
        await AppDbContext.SaveChangesAsync();
        return teachers;
    }

    protected static Teacher[] CreateTeachers(int count)
        => Enumerable.Range(0, count)
                     .Select(_ => CreateTeacher())
                     .ToArray();

    protected static Teacher CreateTeacher()
        => new()
        {
            Name = $"Teacher {Guid.NewGuid()}"
        };
}

using Domain.Database;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace Tests.Internal.Abstracts;

public abstract class AppDbContextTestsBase
{
    private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder()
        .Build();

    // Properties
    protected AppDbContext AppDbContext { get; set; }

    // Methods
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        await _sqlContainer.StartAsync();
        var options = new DbContextOptionsBuilder<AppDbContext>()
          .UseSqlServer(_sqlContainer.GetConnectionString())
          .EnableDetailedErrors()
          .EnableSensitiveDataLogging()
          .Options;

        AppDbContext = new AppDbContext(options, loggerFactory: null);
        await AppDbContext.Database.EnsureCreatedAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDownAsync()
    {
        if (AppDbContext is not null)
        {
            await AppDbContext.DisposeAsync();
        }

        await _sqlContainer.StopAsync();
    }

    [SetUp]
    public Task SetUpAsync()
        => AppDbContext.Database.ExecuteSqlAsync($"DELETE FROM Teachers");

    protected static Teacher[] GenerateTeachers(int count)
        => Enumerable
            .Range(0, count)
            .Select(_ => GenerateTeacher())
            .ToArray();

    protected static Teacher GenerateTeacher()
        => new()
        {
            Name = $"Teacher {Guid.NewGuid()}"
        };

    protected async Task<Teacher[]> InsertTeachersAsync(int count)
    {
        var teachers = GenerateTeachers(count);
        await AppDbContext.AddRangeAsync(teachers);
        await AppDbContext.SaveChangesAsync();
        return teachers;
    }
}

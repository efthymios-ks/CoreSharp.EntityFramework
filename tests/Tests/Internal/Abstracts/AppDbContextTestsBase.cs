using Domain.Database;
using Microsoft.EntityFrameworkCore;

namespace Tests.Internal.Abstracts;

public abstract class AppDbContextTestsBase
{
    // Properties
    internal static AppDbContext AppDbContext { get; set; }

    // Methods 
    [SetUp]
    public async Task SetUpAsync()
    {
        if (AppDbContext is null || AppDbContext.IsDisposed)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
              .UseSqlServer(MsSqlContainerSetup.SqlConnectionString)
              .EnableDetailedErrors()
              .EnableSensitiveDataLogging()
              .Options;
            AppDbContext = new AppDbContext(options, loggerFactory: null);
            await AppDbContext.Database.EnsureCreatedAsync();
        }

        await AppDbContext.Database.ExecuteSqlAsync($"DELETE FROM Teachers");
    }

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

    protected static async Task<Teacher[]> InsertTeachersAsync(int count)
    {
        var teachers = GenerateTeachers(count);
        await AppDbContext.AddRangeAsync(teachers);
        await AppDbContext.SaveChangesAsync();
        return teachers;
    }
}

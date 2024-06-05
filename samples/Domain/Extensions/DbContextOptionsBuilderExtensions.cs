using Domain.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Domain.Extensions;

/// <summary>
/// <see cref="DbContextOptionsBuilder"/> extensions.
/// </summary>
internal static class DbContextOptionsBuilderExtensions
{
    // Methods
    public static DbContextOptionsBuilder ConfigureSql(
        this DbContextOptionsBuilder optionsBuilder,
        ILoggerFactory loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder);

        var databaseName = $"{nameof(AppDbContext)}_{DateTime.Now.ToFileTimeUtc()}";
        optionsBuilder.UseInMemoryDatabase(databaseName: databaseName);
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();
        optionsBuilder.UseLoggerFactory(loggerFactory);

        return optionsBuilder;
    }
}

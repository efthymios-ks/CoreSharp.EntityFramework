using Microsoft.EntityFrameworkCore;
using System;

namespace Domain.Extensions;

/// <summary>
/// <see cref="DbContextOptionsBuilder"/> extensions.
/// </summary>
internal static class DbContextOptionsBuilderExtensions
{
    // Methods
    public static DbContextOptionsBuilder ConfigureSql(this DbContextOptionsBuilder optionsBuilder)
    {
        _ = optionsBuilder ?? throw new ArgumentNullException(nameof(optionsBuilder));

        optionsBuilder.UseSqlServer(Configuration.SqlConnectionString);
        optionsBuilder.EnableSensitiveDataLogging();

        return optionsBuilder;
    }
}

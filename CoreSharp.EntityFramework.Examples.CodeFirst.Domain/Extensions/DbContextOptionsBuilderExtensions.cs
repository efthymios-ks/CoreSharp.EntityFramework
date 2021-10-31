using Microsoft.EntityFrameworkCore;
using System;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Extensions
{
    /// <summary>
    /// <see cref="DbContextOptionsBuilder"/> extensions.
    /// </summary>
    internal static class DbContextOptionsBuilderExtensions
    {
        //Methods
        public static DbContextOptionsBuilder ConfigureSchoolDbContext(this DbContextOptionsBuilder optionsBuilder, string connectionString)
        {
            _ = optionsBuilder ?? throw new ArgumentNullException(nameof(optionsBuilder));

            optionsBuilder.UseSqlServer(connectionString);

            return optionsBuilder;
        }
    }
}

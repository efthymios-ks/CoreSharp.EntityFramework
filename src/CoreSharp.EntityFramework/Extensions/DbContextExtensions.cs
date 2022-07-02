using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Extensions;

/// <summary>
/// <see cref="DbContext"/> extensions.
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    /// Rolls back and erases all data
    /// modifications made in active transaction.
    /// </summary>
    public static async Task RollbackAsync(this DbContext dbContext, CancellationToken cancellationToken = default)
    {
        _ = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        var changedEntities = dbContext.ChangeTracker.Entries()
                                                     .Where(e => e.State != EntityState.Unchanged);
        foreach (var entity in changedEntities)
        {
            switch (entity.State)
            {
                case EntityState.Added:
                    entity.State = EntityState.Detached;
                    break;
                case EntityState.Modified:
                case EntityState.Deleted:
                    await entity.ReloadAsync(cancellationToken);
                    entity.State = EntityState.Unchanged;
                    break;
            }
        }
    }
}

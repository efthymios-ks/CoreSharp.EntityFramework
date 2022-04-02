using CoreSharp.EntityFramework.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Extensions
{
    /// <summary>
    /// <see cref="DbSet{TEntity}"/> extensions.
    /// </summary>
    internal static class DbSetExtensions
    {
        public static async Task<IEnumerable<TEntity>> AddManyAsync<TEntity>(this DbSet<TEntity> dbSet, IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            _ = dbSet ?? throw new ArgumentNullException(nameof(dbSet));
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            //Mutate reference to allow EF to write back auto-generated id 
            var mutatedEntities = entities.ToArray();
            await dbSet.AddRangeAsync(mutatedEntities, cancellationToken);
            return mutatedEntities;
        }

        public static async Task<IEnumerable<TEntity>> UpdateManyAsync<TEntity>(this DbSet<TEntity> dbSet, IEnumerable<TEntity> entities)
            where TEntity : class
        {
            _ = dbSet ?? throw new ArgumentNullException(nameof(dbSet));
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            dbSet.UpdateRange(entities);
            return await Task.FromResult(entities);
        }

        public static async Task RemoveManyAsync<TEntity>(this DbSet<TEntity> dbSet, IEnumerable<TEntity> entities)
            where TEntity : class
        {
            _ = dbSet ?? throw new ArgumentNullException(nameof(dbSet));
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            dbSet.RemoveRange(entities);
            await Task.CompletedTask;
        }

        public static async Task<IEnumerable<TEntity>> AddOrUpdateManyAsync<TEntity>(this DbSet<TEntity> dbSet, IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            _ = dbSet ?? throw new ArgumentNullException(nameof(dbSet));
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            //Get all id in single query 
            var idsToLookFor = entities.Select(e => e.Id)
                                       .Distinct();
            var idsFound = await dbSet.Where(e => idsToLookFor.Contains(e.Id))
                                      .Select(e => e.Id)
                                      .AsNoTracking()
                                      .ToArrayAsync(cancellationToken);
            bool EntityExists(TEntity entity)
                => Array.Exists(idsFound, id => Equals(id, entity.Id));

            //Save entities in batches 
            var entitiesToUpdate = entities.Where(EntityExists);
            var entitiesToAdd = entities.Except(entitiesToUpdate);
            var entitiesAdded = await dbSet.AddManyAsync(entitiesToAdd, cancellationToken);
            var entitiesUpdated = await dbSet.UpdateManyAsync(entitiesToUpdate);
            return entitiesAdded.Concat(entitiesUpdated);
        }
    }
}

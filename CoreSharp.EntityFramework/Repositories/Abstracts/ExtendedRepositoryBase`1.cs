using CoreSharp.EntityFramework.Delegates;
using CoreSharp.EntityFramework.Extensions;
using CoreSharp.EntityFramework.Models.Interfaces;
using CoreSharp.EntityFramework.Repositories.Interfaces;
using CoreSharp.Extensions;
using CoreSharp.Models.Pages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Repositories.Abstracts
{
    public abstract class ExtendedRepositoryBase<TEntity> : RepositoryBase<TEntity>, IExtendedRepository<TEntity>
        where TEntity : class, IEntity
    {
        //Constructors
        protected ExtendedRepositoryBase(DbContext dbContext)
            : base(dbContext)
        {
        }

        //Methods 
        public virtual async Task<IEnumerable<TEntity>> AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            //Mutate reference to allow EF to write back auto-generated id 
            var mutatedEntities = entities.ToArray();
            await Table.AddRangeAsync(mutatedEntities, cancellationToken);
            return mutatedEntities;
        }

        public virtual async Task<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            await Task.CompletedTask;
            Table.UpdateRange(entities);
            return entities;
        }

        public virtual async Task RemoveAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            await Task.CompletedTask;
            Table.RemoveRange(entities);
        }

        public virtual async Task RemoveAsync(object key, CancellationToken cancellationToken = default)
        {
            _ = key ?? throw new ArgumentNullException(nameof(key));

            var entity = await GetAsync(key, cancellationToken: cancellationToken);
            _ = entity ?? throw new KeyNotFoundException($"Could not find entity with key=`{key}`.");
            await base.RemoveAsync(entity, cancellationToken);
        }

        public virtual async Task<bool> ExistsAsync(object key, CancellationToken cancellationToken = default)
        {
            _ = key ?? throw new ArgumentNullException(nameof(key));

            return await ExistsAsync(q => q.Where(e => Equals(e.Id, key)), cancellationToken);
        }

        public virtual async Task<bool> ExistsAsync(Query<TEntity> navigation = null, CancellationToken cancellationToken = default)
        {
            var query = NavigateTable(navigation);
            var foundIds = await query.Select(e => e.Id)
                                      .Take(1)
                                      .AsNoTracking()
                                      .ToArrayAsync(cancellationToken);
            return foundIds.Length > 0;
        }

        public virtual async Task<long> CountAsync(Query<TEntity> navigation = null, CancellationToken cancellationToken = default)
        {
            navigation ??= q => q;
            var query = NavigateTable(navigation).AsNoTracking();
            return await query.LongCountAsync(cancellationToken);
        }

        public virtual async Task<TEntity> AddOrUpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _ = entity ?? throw new ArgumentNullException(nameof(entity));

            if (await ExistsAsync(entity.Id, cancellationToken))
                return await UpdateAsync(entity, cancellationToken);
            else
                return await AddAsync(entity, cancellationToken);
        }

        public virtual async Task<IEnumerable<TEntity>> AddOrUpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            //Get all id in single query 
            var idsToLookFor = entities.Select(e => e.Id)
                                       .Distinct();
            var idsFound = await Table.Where(e => idsToLookFor.Contains(e.Id))
                                      .Select(e => e.Id)
                                      .AsNoTracking()
                                      .ToArrayAsync(cancellationToken);
            bool EntityExists(TEntity entity)
                => Array.Exists(idsFound, id => Equals(id, entity.Id));

            //Save entities in batches 
            var entitiesToUpdate = entities.Where(EntityExists);
            var entitiesToAdd = entities.Except(entitiesToUpdate);
            var entitiesAdded = await AddAsync(entitiesToAdd, cancellationToken);
            var entitiesUpdated = await UpdateAsync(entitiesToUpdate, cancellationToken);
            return entitiesAdded.Concat(entitiesUpdated);
        }

        public virtual async Task<Page<TEntity>> GetPageAsync(int pageNumber, int pageSize, Query<TEntity> navigation = null, CancellationToken cancellationToken = default)
        {
            if (pageNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumber));
            else if (pageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(pageSize));

            navigation ??= q => q;
            var query = NavigateTable(navigation);
            return await query.GetPageAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}

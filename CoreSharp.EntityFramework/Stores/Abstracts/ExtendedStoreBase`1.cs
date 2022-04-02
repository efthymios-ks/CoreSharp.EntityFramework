﻿using CoreSharp.EntityFramework.Delegates;
using CoreSharp.EntityFramework.Extensions;
using CoreSharp.EntityFramework.Models.Interfaces;
using CoreSharp.EntityFramework.Stores.Interfaces;
using CoreSharp.Models.Pages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Stores.Abstracts
{
    public abstract class ExtendedStoreBase<TEntity> : StoreBase<TEntity>, IExtendedStore<TEntity>
        where TEntity : class, IEntity
    {
        //Constructors
        protected ExtendedStoreBase(DbContext dbContext)
            : base(dbContext)
        {
        }

        //Methods 
        public virtual async Task<IEnumerable<TEntity>> AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            var addedEntities = await Table.AddManyAsync(entities, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
            return addedEntities;
        }

        public virtual async Task<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            var updatedEntities = await Table.UpdateManyAsync(entities);
            await Context.SaveChangesAsync(cancellationToken);
            return updatedEntities;
        }

        public virtual async Task RemoveAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            await Table.RemoveManyAsync(entities);
            await Context.SaveChangesAsync(cancellationToken);
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

            var finalEntities = await Table.AddOrUpdateManyAsync(entities, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
            return finalEntities;
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

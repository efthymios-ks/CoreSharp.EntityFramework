﻿using CoreSharp.EntityFramework.Delegates;
using CoreSharp.EntityFramework.Extensions;
using CoreSharp.EntityFramework.Models.Interfaces;
using CoreSharp.EntityFramework.Repositories.Interfaces;
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

            var result = new HashSet<TEntity>();
            foreach (var entity in entities)
                result.Add(await AddAsync(entity, cancellationToken));
            return result;
        }

        public virtual async Task<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            var result = new HashSet<TEntity>();
            foreach (var entity in entities)
                result.Add(await UpdateAsync(entity, cancellationToken));
            return result;
        }

        public virtual async Task RemoveAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            foreach (var entity in entities)
                await RemoveAsync(entity, cancellationToken);
        }

        public virtual async Task RemoveByKeyAsync(object key, CancellationToken cancellationToken = default)
        {
            _ = key ?? throw new ArgumentNullException(nameof(key));

            var entity = await GetAsync(key, cancellationToken: cancellationToken);
            _ = entity ?? throw new KeyNotFoundException($"Could not find entity with key=`{key}`.");
            await base.RemoveAsync(entity, cancellationToken);
        }

        public virtual async Task<bool> ExistsAsync(object key, CancellationToken cancellationToken = default)
        {
            _ = key ?? throw new ArgumentNullException(nameof(key));

            var foundItem = await GetAsync(key, cancellationToken: cancellationToken);
            return foundItem is not null;
        }

        public virtual async Task<bool> ExistsAsync(Query<TEntity> navigation = null, CancellationToken cancellationToken = default)
        {
            navigation ??= q => q;
            IQueryable<TEntity> NavigateOne(IQueryable<TEntity> queryable) => navigation(queryable).Take(1);
            var foundItems = await GetAsync(NavigateOne, cancellationToken);
            return foundItems.Any();
        }

        public virtual async Task<long> CountAsync(Query<TEntity> navigation = null, CancellationToken cancellationToken = default)
        {
            navigation ??= q => q;
            var query = NavigateTable(navigation);
            return await query.LongCountAsync(cancellationToken);
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

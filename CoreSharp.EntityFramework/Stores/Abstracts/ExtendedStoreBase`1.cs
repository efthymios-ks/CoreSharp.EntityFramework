using CoreSharp.EntityFramework.Models.Interfaces;
using CoreSharp.EntityFramework.Stores.Interfaces;
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
        protected ExtendedStoreBase(DbContext dbContext) : base(dbContext)
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

        public async Task RemoveByKeyAsync(object key, CancellationToken cancellationToken = default)
        {
            _ = key ?? throw new ArgumentNullException(nameof(key));

            var entity = await GetAsync(key, cancellationToken: cancellationToken);
            _ = entity ?? throw new KeyNotFoundException($"Could not find entity with key=`{key}`.");
            await RemoveAsync(entity, cancellationToken);
        }

        public async Task<bool> ExistsAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> navigation = null, CancellationToken cancellationToken = default)
        {
            navigation ??= q => q;
            IQueryable<TEntity> NavigateOne(IQueryable<TEntity> queryable) => navigation(queryable).Take(1);
            var items = await GetAsync(NavigateOne, cancellationToken);
            return items.Any();
        }
    }
}

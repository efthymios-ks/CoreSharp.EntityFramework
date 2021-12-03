using CoreSharp.EntityFramework.Models.Interfaces;
using CoreSharp.EntityFramework.Stores.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
    }
}

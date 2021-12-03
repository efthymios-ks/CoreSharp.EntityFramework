using CoreSharp.EntityFramework.Models.Interfaces;
using CoreSharp.EntityFramework.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Repositories.Abstracts
{
    public abstract class ExtendedRepositoryBase<TEntity> : RepositoryBase<TEntity>, IExtendedRepository<TEntity>
        where TEntity : class, IEntity
    {
        //Constructors
        protected ExtendedRepositoryBase(DbContext dbContext) : base(dbContext)
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

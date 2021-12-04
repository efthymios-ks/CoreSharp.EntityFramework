using CoreSharp.EntityFramework.Models.Interfaces;
using CoreSharp.EntityFramework.Repositories.Abstracts;
using CoreSharp.EntityFramework.Stores.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Stores.Abstracts
{
    public abstract class StoreBase<TEntity> : RepositoryBase<TEntity>, IStore<TEntity>
        where TEntity : class, IEntity
    {
        //Constructors
        protected StoreBase(DbContext dbContext) : base(dbContext)
        {
        }

        //Methods 
        public override async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _ = entity ?? throw new ArgumentNullException(nameof(entity));

            var createdEntity = await base.AddAsync(entity, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
            return createdEntity;
        }

        public override async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _ = entity ?? throw new ArgumentNullException(nameof(entity));

            var updatedEntity = await base.UpdateAsync(entity, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
            return updatedEntity;
        }

        public override async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _ = entity ?? throw new ArgumentNullException(nameof(entity));

            await base.RemoveAsync(entity, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
        }
    }
}

using CoreSharp.EntityFramework.Extensions;
using CoreSharp.EntityFramework.Models.Interfaces;
using CoreSharp.EntityFramework.Repositories.Abstracts;
using CoreSharp.EntityFramework.Store.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Store.Abstracts
{
    public abstract class StoreBase<TEntity> : RepositoryBase<TEntity>, IStore<TEntity>
        where TEntity : class, IEntity
    {
        //Constructors
        protected StoreBase(DbContext dbContext) : base(dbContext)
        {
        }

        //Methods
        public async Task CommitAsync(CancellationToken cancellationToken = default)
            => await Context.SaveChangesAsync(cancellationToken);

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
            => await Context.RollbackAsync(cancellationToken);

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Context?.Dispose();
        }
    }
}

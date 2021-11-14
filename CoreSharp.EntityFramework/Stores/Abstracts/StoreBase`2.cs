using CoreSharp.EntityFramework.Models.Interfaces;
using CoreSharp.EntityFramework.Stores.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Stores.Abstracts
{
    /// <inheritdoc />
    public abstract class StoreBase<TEntity, TKey> : StoreBase<TEntity>, IStore<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {
        //Constructors
        protected StoreBase(DbContext dbContext) : base(dbContext)
        {
        }

        //Methods
        public Task<TEntity> GetAsync(
            TKey key,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> navigation = null,
            CancellationToken cancellationToken = default)
            => base.GetAsync(key, navigation, cancellationToken);
    }
}

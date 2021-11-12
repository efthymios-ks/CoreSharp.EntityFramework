using CoreSharp.EntityFramework.Models.Interfaces;
using CoreSharp.EntityFramework.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Repositories.Abstracts
{
    public abstract class RepositoryBase<TEntity, TKey> : RepositoryBase<TEntity>, IRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {
        //Constructors
        protected RepositoryBase(DbContext dbContext) : base(dbContext)
        {
        }

        //Methods 
        public async virtual Task<TEntity> GetAsync(
            TKey key,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> navigation = null,
            CancellationToken cancellationToken = default)
            => await base.GetAsync(key, navigation, cancellationToken);
    }
}

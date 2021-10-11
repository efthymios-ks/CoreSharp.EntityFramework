using CoreSharp.EntityFramework.Models.Abstracts;
using CoreSharp.EntityFramework.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Repositories.Abstracts
{
    public abstract class BaseRepository<TEntity, TKey> : BaseRepository<TEntity>, IRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
    {
        //Constructors
        protected BaseRepository(DbContext context) : base(context)
        {
        }

        //Methods 
        public async virtual Task<TEntity> GetAsync(TKey key, Func<IQueryable<TEntity>, IQueryable<TEntity>> navigation = null)
            => await GetAsync(key as object, navigation);
    }
}

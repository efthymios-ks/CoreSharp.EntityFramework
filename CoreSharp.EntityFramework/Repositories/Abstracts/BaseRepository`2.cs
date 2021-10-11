using CoreSharp.EntityFramework.Models.Abstracts;
using CoreSharp.EntityFramework.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Repositories.Abstracts
{
    public abstract class BaseRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        //Constructors
        protected BaseRepository(DbContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Table = Context.Set<TEntity>();
        }

        //Properties 
        protected DbContext Context { get; }
        protected DbSet<TEntity> Table { get; }

        //Methods 
        public async virtual Task<TEntity> GetAsync(TKey key, Func<IQueryable<TEntity>, IQueryable<TEntity>> navigation = null)
        {
            _ = key ?? throw new ArgumentNullException(nameof(key));

            var entities = await GetAsync(i => Equals(i.Id, key), navigation);
            return entities.SingleOrDefault();
        }

        public async virtual Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IQueryable<TEntity>> navigation = null)
        {
            var query = Table.AsQueryable();
            if (navigation is not null)
                query = navigation(query);
            if (filter is not null)
                query = query.Where(filter);

            return await query.ToArrayAsync();
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            _ = entity ?? throw new ArgumentNullException(nameof(entity));

            await Table.AddAsync(entity);
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            _ = entity ?? throw new ArgumentNullException(nameof(entity));

            Table.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;

            await Task.CompletedTask;
        }

        public virtual async Task RemoveAsync(TEntity entity)
        {
            _ = entity ?? throw new ArgumentNullException(nameof(entity));

            Table.Remove(entity);

            await Task.CompletedTask;
        }
    }
}

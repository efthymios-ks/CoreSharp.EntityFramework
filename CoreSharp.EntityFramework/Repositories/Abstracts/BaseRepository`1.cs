using CoreSharp.EntityFramework.Models.Interfaces;
using CoreSharp.EntityFramework.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Repositories.Abstracts
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
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
        public async virtual Task<TEntity> GetAsync(object key, Func<IQueryable<TEntity>, IQueryable<TEntity>> navigation = null)
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

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            _ = entity ?? throw new ArgumentNullException(nameof(entity));

            return (await Table.AddAsync(entity).AsTask()).Entity;
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            _ = entity ?? throw new ArgumentNullException(nameof(entity));
            await Task.CompletedTask;

            Table.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;

            return entity;
        }

        public virtual async Task RemoveAsync(TEntity entity)
        {
            _ = entity ?? throw new ArgumentNullException(nameof(entity));
            await Task.CompletedTask;

            Table.Remove(entity);
        }
    }
}

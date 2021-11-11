using CoreSharp.EntityFramework.Models.Interfaces;
using CoreSharp.EntityFramework.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Repositories.Abstracts
{
    public abstract class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        //Constructors
        protected RepositoryBase(DbContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Table = Context.Set<TEntity>();
        }

        //Properties 
        protected DbContext Context { get; }
        protected DbSet<TEntity> Table { get; }

        //Methods 
        public async virtual Task<TEntity> GetAsync(
            object key,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> navigation = null,
            CancellationToken cancellationToken = default)
        {
            _ = key ?? throw new ArgumentNullException(nameof(key));

            var query = Table.AsQueryable();
            if (navigation is not null)
                query = navigation(query);
            return await query.SingleOrDefaultAsync(i => Equals(i.Id, key), cancellationToken: cancellationToken);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAsync(
            Func<IQueryable<TEntity>, IQueryable<TEntity>> navigation = null,
            CancellationToken cancellationToken = default)
        {
            var query = Table.AsQueryable();
            if (navigation is not null)
                query = navigation(query);

            return await query.ToArrayAsync(cancellationToken: cancellationToken);
        }

        public virtual async Task<TEntity> AddAsync(
            TEntity entity,
            CancellationToken cancellationToken = default)
        {
            _ = entity ?? throw new ArgumentNullException(nameof(entity));

            return (await Table.AddAsync(entity, cancellationToken)).Entity;
        }

        public virtual async Task<TEntity> UpdateAsync(
            TEntity entity,
            CancellationToken cancellationToken = default)
        {
            _ = entity ?? throw new ArgumentNullException(nameof(entity));
            await Task.CompletedTask;

            Table.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;

            return entity;
        }

        public virtual async Task RemoveAsync(
            TEntity entity,
            CancellationToken cancellationToken = default)
        {
            _ = entity ?? throw new ArgumentNullException(nameof(entity));
            await Task.CompletedTask;

            Table.Remove(entity);
        }
    }
}

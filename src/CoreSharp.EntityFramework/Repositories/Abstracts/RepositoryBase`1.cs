using CoreSharp.EntityFramework.Delegates;
using CoreSharp.EntityFramework.Entities.Interfaces;
using CoreSharp.EntityFramework.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Repositories.Abstracts;

public abstract class RepositoryBase<TEntity> : IRepository<TEntity>
    where TEntity : class, IEntity
{
    // Constructors
    protected RepositoryBase(DbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        Context = dbContext;
        Table = Context.Set<TEntity>();
    }

    // Properties 
    protected DbContext Context { get; }

    protected DbSet<TEntity> Table { get; }

    // Methods 
    public virtual async Task<TEntity> GetAsync(
        object key,
        Query<TEntity> navigation = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);

        var query = NavigateTable(navigation);
        return await query.SingleOrDefaultAsync(e => Equals(e.Id, key), cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAsync(
        Query<TEntity> navigation = null,
        CancellationToken cancellationToken = default)
    {
        var query = NavigateTable(navigation);
        return await query.ToArrayAsync(cancellationToken);
    }

    public virtual async Task<TEntity> AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await Table.AddAsync(entity, cancellationToken);
        return entity;
    }

    public virtual async Task<TEntity> UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        Table.Attach(entity);
        return await Task.FromResult(entity);
    }

    public virtual async Task RemoveAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        Table.Remove(entity);
        await Task.CompletedTask;
    }

    protected IQueryable<TEntity> NavigateTable(Query<TEntity> navigation)
    {
        navigation ??= queryable => queryable;
        var query = Table.AsQueryable();
        return navigation(query);
    }
}

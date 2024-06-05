using CoreSharp.EntityFramework.Delegates;
using CoreSharp.EntityFramework.Entities.Interfaces;
using CoreSharp.EntityFramework.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Repositories.Abstracts;

public abstract class RepositoryBase<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
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
    public virtual Task<TEntity?> GetAsync(
        TKey key,
        Query<TEntity>? navigation = null,
        CancellationToken cancellationToken = default)
    {
        var queyrable = NavigateTable(navigation);
        return queyrable.SingleOrDefaultAsync(e => Equals(e.Id, key), cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAsync(
        Query<TEntity>? navigation = null,
        CancellationToken cancellationToken = default)
    {
        var queryable = NavigateTable(navigation);
        return await queryable.ToArrayAsync(cancellationToken);
    }

    public virtual async Task<TEntity> AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await Table.AddAsync(entity, cancellationToken);
        return entity;
    }

    public virtual Task<TEntity> UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        Table.Attach(entity);
        return Task.FromResult(entity);
    }

    public virtual Task RemoveAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        Table.Remove(entity);
        return Task.CompletedTask;
    }

    protected IQueryable<TEntity> NavigateTable(Query<TEntity>? query)
    {
        query ??= queryable => queryable;
        var queryable = Table.AsQueryable();
        return query(queryable);
    }
}

using CoreSharp.EntityFramework.Delegates;
using CoreSharp.EntityFramework.Entities.Interfaces;
using CoreSharp.EntityFramework.Extensions;
using CoreSharp.EntityFramework.Repositories.Interfaces;
using CoreSharp.Extensions;
using CoreSharp.Models.Pages;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Repositories.Abstracts;

public abstract class ExtendedRepositoryBase<TEntity, TKey>(DbContext dbContext)
    : RepositoryBase<TEntity, TKey>(dbContext), IExtendedRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{

    // Methods 
    public virtual Task<IEnumerable<TEntity>> AddAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);

        return Table.AddManyAsync<TEntity, TKey>(entities, cancellationToken);
    }

    public virtual Task<IEnumerable<TEntity>> UpdateAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);

        return Table.AttachManyAsync<TEntity, TKey>(entities);
    }

    public virtual Task RemoveAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);

        return Table.RemoveManyAsync<TEntity, TKey>(entities);
    }

    public virtual Task RemoveAsync(
        TKey key,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);

        return Table.RemoveByKeyAsync(key, cancellationToken);
    }

    public virtual Task<bool> ExistsAsync(
        TKey key,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);

        return ExistsAsync(q => q.Where(e => Equals(e.Id, key)), cancellationToken);
    }

    public virtual async Task<bool> ExistsAsync(
        Query<TEntity>? navigation = null,
        CancellationToken cancellationToken = default)
    {
        var query = NavigateTable(navigation);
        var foundIds = await query
            .Select(e => e.Id)
            .Take(1)
            .ToArrayAsync(cancellationToken);
        return foundIds.Length > 0;
    }

    public virtual Task<long> CountAsync(
        Query<TEntity>? navigation = null,
        CancellationToken cancellationToken = default)
    {
        var query = NavigateTable(navigation).AsNoTracking();
        return query.LongCountAsync(cancellationToken);
    }

    public virtual async Task<TEntity> AddOrUpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return await ExistsAsync(entity.Id, cancellationToken)
            ? await UpdateAsync(entity, cancellationToken)
            : await AddAsync(entity, cancellationToken);
    }

    public virtual Task<IEnumerable<TEntity>> AddOrUpdateAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);

        return Table.AddOrAttachManyAsync<TEntity, TKey>(entities, cancellationToken);
    }

    public virtual async Task<TEntity> AddIfNotExistAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var existing = await GetAsync(entity.Id, cancellationToken: cancellationToken);
        return existing ?? await AddAsync(entity, cancellationToken);
    }

    public virtual Task<IEnumerable<TEntity>> AddIfNotExistAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);

        return Table.AddManyIfNotExistAsync<TEntity, TKey>(entities, cancellationToken);
    }

    public virtual async Task<TEntity> UpdateIfExistAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        if (await ExistsAsync(entity.Id, cancellationToken))
        {
            entity = await UpdateAsync(entity, cancellationToken);
        }

        return entity;
    }

    public virtual Task<IEnumerable<TEntity>> UpdateIfExistAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);

        return Table.AttachManyIfExistAsync<TEntity, TKey>(entities, cancellationToken);
    }

    public virtual Task<Page<TEntity>> GetPageAsync(
        int pageNumber,
        int pageSize,
        Query<TEntity>? navigation = null,
        CancellationToken cancellationToken = default)
    {
        var query = NavigateTable(navigation);
        return query.GetPageAsync(pageNumber, pageSize, cancellationToken);
    }
}

using CoreSharp.EntityFramework.Delegates;
using CoreSharp.EntityFramework.Entities.Interfaces;
using CoreSharp.EntityFramework.Extensions;
using CoreSharp.EntityFramework.Stores.Interfaces;
using CoreSharp.Models.Pages;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Stores.Abstracts;

public abstract class ExtendedStoreBase<TEntity, TKey>(DbContext dbContext)
    : StoreBase<TEntity, TKey>(dbContext), IExtendedStore<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{

    // Methods 
    public virtual async Task<IEnumerable<TEntity>> AddAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);

        var addedEntities = await Table.AddManyAsync<TEntity, TKey>(entities, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        return addedEntities;
    }

    public virtual async Task<IEnumerable<TEntity>> UpdateAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);

        var updatedEntities = await Table.AttachManyAsync<TEntity, TKey>(entities);
        await Context.SaveChangesAsync(cancellationToken);
        return updatedEntities;
    }

    public virtual async Task RemoveAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);

        await Table.RemoveManyAsync<TEntity, TKey>(entities);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task RemoveAsync(
        TKey key,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);

        await Table.RemoveByKeyAsync(key, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
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

    public virtual async Task<long> CountAsync(
        Query<TEntity>? navigation = null,
        CancellationToken cancellationToken = default)
    {
        var query = NavigateTable(navigation).AsNoTracking();
        return await query.LongCountAsync(cancellationToken);
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

    public virtual async Task<IEnumerable<TEntity>> AddOrUpdateAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);

        var entitiesAddedorUpdated = await Table.AddOrAttachManyAsync<TEntity, TKey>(entities, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        return entitiesAddedorUpdated;
    }

    public virtual async Task<TEntity> AddIfNotExistAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var existing = await GetAsync(entity.Id, cancellationToken: cancellationToken);
        return existing ?? await AddAsync(entity, cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> AddIfNotExistAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);

        var entitiesAdded = await Table.AddManyIfNotExistAsync<TEntity, TKey>(entities, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        return entitiesAdded;
    }

    public virtual async Task<TEntity> UpdateIfExistAsync(
        TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        if (await ExistsAsync(entity.Id, cancellationToken))
        {
            entity = await UpdateAsync(entity, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
        }

        return entity;
    }

    public virtual async Task<IEnumerable<TEntity>> UpdateIfExistAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);

        var entitiesUpdated = await Table.AttachManyIfExistAsync<TEntity, TKey>(entities, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        return entitiesUpdated;
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

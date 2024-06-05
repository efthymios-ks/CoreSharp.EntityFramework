using CoreSharp.EntityFramework.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace CoreSharp.EntityFramework.Extensions;

/// <summary>
/// <see cref="DbSet{TEntity}"/> internal extensions.
/// </summary>
internal static class DbSetExtensions
{
    public static async Task<IEnumerable<TEntity>> AddManyAsync<TEntity, TKey>(
        this DbSet<TEntity> dbSet,
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity<TKey>
    {
        ArgumentNullException.ThrowIfNull(dbSet);
        ArgumentNullException.ThrowIfNull(entities);

        // Mutate reference to allow EF to write back auto-generated id 
        var mutatedEntities = entities.ToArray();
        await dbSet.AddRangeAsync(mutatedEntities, cancellationToken);
        return mutatedEntities;
    }

    public static Task<IEnumerable<TEntity>> AttachManyAsync<TEntity, TKey>(
        this DbSet<TEntity> dbSet,
        IEnumerable<TEntity> entities)
        where TEntity : class, IEntity<TKey>
    {
        ArgumentNullException.ThrowIfNull(dbSet);
        ArgumentNullException.ThrowIfNull(entities);

        dbSet.AttachRange(entities);
        return Task.FromResult(entities);
    }

    public static Task<IEnumerable<TEntity>> UpdateManyAsync<TEntity, TKey>(
        this DbSet<TEntity> dbSet,
        IEnumerable<TEntity> entities)
        where TEntity : class, IEntity<TKey>
    {
        ArgumentNullException.ThrowIfNull(dbSet);
        ArgumentNullException.ThrowIfNull(entities);

        dbSet.UpdateRange(entities);
        return Task.FromResult(entities);
    }

    public static async Task RemoveManyAsync<TEntity, TKey>(
        this DbSet<TEntity> dbSet,
        IEnumerable<TEntity> entities)
        where TEntity : class, IEntity<TKey>
    {
        ArgumentNullException.ThrowIfNull(dbSet);
        ArgumentNullException.ThrowIfNull(entities);

        dbSet.RemoveRange(entities);
        await Task.CompletedTask;
    }

    public static async Task<bool> RemoveByKeyAsync<TEntity, TKey>(
        this DbSet<TEntity> dbSet,
        TKey key,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity<TKey>
    {
        ArgumentNullException.ThrowIfNull(dbSet);
        ArgumentNullException.ThrowIfNull(key);

        var entityFound = await dbSet.FirstOrDefaultAsync(entity => Equals(entity.Id, key), cancellationToken);
        if (entityFound is null)
        {
            return false;
        }

        dbSet.Remove(entityFound);
        return true;
    }

    public static Task<IEnumerable<TEntity>> AddManyIfNotExistAsync<TEntity, TKey>(
        this DbSet<TEntity> dbSet,
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity<TKey>
    {
        ArgumentNullException.ThrowIfNull(dbSet);
        ArgumentNullException.ThrowIfNull(entities);

        return dbSet.AddOrUpdateManyInternalAsync<TEntity, TKey>(entities, AddAction, DiscardUpdateAction, cancellationToken);

        Task<IEnumerable<TEntity>> AddAction(IEnumerable<TEntity> entitiesToAdd)
            => dbSet.AddManyAsync<TEntity, TKey>(entitiesToAdd, cancellationToken);

        Task<IEnumerable<TEntity>> DiscardUpdateAction(IEnumerable<TEntity> _)
            => Task.FromResult(Enumerable.Empty<TEntity>());
    }

    public static Task<IEnumerable<TEntity>> AttachManyIfExistAsync<TEntity, TKey>(
        this DbSet<TEntity> dbSet,
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity<TKey>
    {
        ArgumentNullException.ThrowIfNull(dbSet);
        ArgumentNullException.ThrowIfNull(entities);

        return dbSet.AddOrUpdateManyInternalAsync<TEntity, TKey>(entities, DiscardAddAction, UpdateAction, cancellationToken);

        Task<IEnumerable<TEntity>> DiscardAddAction(IEnumerable<TEntity> _)
            => Task.FromResult(Enumerable.Empty<TEntity>());

        Task<IEnumerable<TEntity>> UpdateAction(IEnumerable<TEntity> entitiesToUpdate)
            => dbSet.AttachManyAsync<TEntity, TKey>(entitiesToUpdate);
    }

    public static Task<IEnumerable<TEntity>> UpdateManyIfExistAsync<TEntity, TKey>(
        this DbSet<TEntity> dbSet,
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity<TKey>
    {
        ArgumentNullException.ThrowIfNull(dbSet);
        ArgumentNullException.ThrowIfNull(entities);

        return dbSet.AddOrUpdateManyInternalAsync<TEntity, TKey>(entities, DiscardAddAction, UpdateAction, cancellationToken);

        Task<IEnumerable<TEntity>> DiscardAddAction(IEnumerable<TEntity> _)
            => Task.FromResult(Enumerable.Empty<TEntity>());

        Task<IEnumerable<TEntity>> UpdateAction(IEnumerable<TEntity> entitiesToUpdate)
            => dbSet.UpdateManyAsync<TEntity, TKey>(entitiesToUpdate);
    }

    public static Task<IEnumerable<TEntity>> AddOrAttachManyAsync<TEntity, TKey>(
        this DbSet<TEntity> dbSet,
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity<TKey>
    {
        ArgumentNullException.ThrowIfNull(dbSet);
        ArgumentNullException.ThrowIfNull(entities);

        return dbSet.AddOrUpdateManyInternalAsync<TEntity, TKey>(entities, AddAction, UpdateAction, cancellationToken);

        Task<IEnumerable<TEntity>> AddAction(IEnumerable<TEntity> entitiesToAdd)
            => dbSet.AddManyAsync<TEntity, TKey>(entitiesToAdd, cancellationToken);

        Task<IEnumerable<TEntity>> UpdateAction(IEnumerable<TEntity> entitiesToUpdate)
            => dbSet.AttachManyAsync<TEntity, TKey>(entitiesToUpdate);
    }

    public static Task<IEnumerable<TEntity>> AddOrUpdateManyAsync<TEntity, TKey>(
        this DbSet<TEntity> dbSet,
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity<TKey>
    {
        ArgumentNullException.ThrowIfNull(dbSet);
        ArgumentNullException.ThrowIfNull(entities);

        return dbSet.AddOrUpdateManyInternalAsync<TEntity, TKey>(entities, AddAction, UpdateAction, cancellationToken);

        Task<IEnumerable<TEntity>> AddAction(IEnumerable<TEntity> entitiesToAdd)
            => dbSet.AddManyAsync<TEntity, TKey>(entitiesToAdd, cancellationToken);

        Task<IEnumerable<TEntity>> UpdateAction(IEnumerable<TEntity> entitiesToUpdate)
            => dbSet.UpdateManyAsync<TEntity, TKey>(entitiesToUpdate);
    }

    private static async Task<IEnumerable<TEntity>> AddOrUpdateManyInternalAsync<TEntity, TKey>(
        this DbSet<TEntity> dbSet,
        IEnumerable<TEntity> entities,
        Func<IEnumerable<TEntity>, Task<IEnumerable<TEntity>>> addAction,
        Func<IEnumerable<TEntity>, Task<IEnumerable<TEntity>>> updateAction,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity<TKey>
    {
        // Get all ids in single query. 
        var idsToLookFor = entities
            .Select(entity => entity.Id)
            .Distinct()
            .ToArray();

        var idsFound = await dbSet
            .AsNoTracking()
            .Where(entity => idsToLookFor.Contains(entity.Id))
            .Select(entity => entity.Id)
            .ToArrayAsync(cancellationToken);

        // Save entities in batches.
        var entitiesToUpdate = entities.Where(EntityExists);
        var entitiesToAdd = entities.Except(entitiesToUpdate);
        var entitiesAdded = await addAction(entitiesToAdd);
        var entitiesUpdated = await updateAction(entitiesToUpdate);
        return entitiesAdded.Concat(entitiesUpdated);

        bool EntityExists(TEntity entity)
            => Array.Exists(idsFound, id => Equals(id, entity.Id));
    }
}

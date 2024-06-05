using CoreSharp.EntityFramework.Delegates;
using CoreSharp.EntityFramework.Entities.Interfaces;
using CoreSharp.Models.Pages;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Repositories.Interfaces;

/// <inheritdoc />
public interface IExtendedRepository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    // Methods 
    /// <inheritdoc cref="IRepository{TEntity, TKey}.AddAsync(TEntity, CancellationToken)" />
    Task<IEnumerable<TEntity>> AddAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default);

    /// <inheritdoc cref="IRepository{TEntity, TKey}.UpdateAsync(TEntity, CancellationToken)" />
    Task<IEnumerable<TEntity>> UpdateAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default);

    /// <inheritdoc cref="IRepository{TEntity, TKey}.RemoveAsync(TEntity, CancellationToken)" />
    Task RemoveAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default);

    /// <inheritdoc cref="IRepository{TEntity, TKey}.RemoveAsync(TEntity, CancellationToken)" />
    Task RemoveAsync(
        TKey key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check any entity exists with given id.
    /// </summary>
    Task<bool> ExistsAsync(
        TKey key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if the subquery returns one or more records.
    /// </summary>
    Task<bool> ExistsAsync(
        Query<TEntity>? navigation = null,
        CancellationToken cancellationToken = default);

    /// <inheritdoc cref="EntityFrameworkQueryableExtensions.CountAsync{TSource}(IQueryable{TSource}, CancellationToken)" />
    Task<long> CountAsync(
        Query<TEntity>? navigation = null,
        CancellationToken cancellationToken = default);

    /// <inheritdoc cref="AddOrUpdateAsync(IEnumerable{TEntity}, CancellationToken)"/>
    Task<TEntity> AddOrUpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds or updates entities by key when
    /// <see cref="DbContext.SaveChangesAsync(CancellationToken)"/> is called.
    /// Equivalent to an "upsert" operation from database terminology.
    /// This method can useful when seeding data using migrations.
    /// </summary>
    Task<IEnumerable<TEntity>> AddOrUpdateAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default);

    /// <inheritdoc cref="AddIfNotExistAsync(IEnumerable{TEntity}, CancellationToken)"/>
    Task<TEntity> AddIfNotExistAsync(
        TEntity entity,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Add non existing entities.
    /// Ignore existing.
    /// <see cref="DbContext.SaveChangesAsync(CancellationToken)"/> is called.
    /// </summary>
    Task<IEnumerable<TEntity>> AddIfNotExistAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default);

    /// <inheritdoc cref="UpdateIfExistAsync(IEnumerable{TEntity}, CancellationToken)"/>
    Task<TEntity> UpdateIfExistAsync(
        TEntity entity,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update existing entities.
    /// Ignore non existing.
    /// <see cref="DbContext.SaveChangesAsync(CancellationToken)"/> is called.
    /// </summary>
    Task<IEnumerable<TEntity>> UpdateIfExistAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Return paged entity collection.
    /// </summary>
    Task<Page<TEntity>> GetPageAsync(
        int pageNumber,
        int pageSize,
        Query<TEntity>? navigation = null,
        CancellationToken cancellationToken = default);
}

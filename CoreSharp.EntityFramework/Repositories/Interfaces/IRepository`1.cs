using CoreSharp.EntityFramework.Delegates;
using CoreSharp.EntityFramework.Entities.Abstracts;
using CoreSharp.EntityFramework.Entities.Interfaces;
using CoreSharp.EntityFramework.Repositories.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Repositories.Interfaces;

/// <summary>
/// Repository interface used for transactional querying.
/// Used with <see cref="IUnitOfWork"/> for commiting transactions.
/// Suggested implementation base with <see cref="RepositoryBase{TEntity, TKey}"/>.
/// </summary>
/// <typeparam name="TEntity">Suggested implementation base with <see cref="EntityBase{TKey}"/>.</typeparam>
/// <typeparam name="TKey">PK type of TEntity.</typeparam>
public interface IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    // Methods
    /// <summary>
    /// Get single entity by given key.
    /// </summary>
    /// <param name="key">Primary key to match.</param>
    /// <param name="query">Optional argument to build query.</param>
    /// <param name="cancellationToken"></param>
    Task<TEntity?> GetAsync(
        TKey key,
        Query<TEntity>? navigation = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all entities.
    /// </summary>
    /// <param name="query">Optional argument to build query.</param>
    /// <param name="cancellationToken"></param>
    Task<IEnumerable<TEntity>> GetAsync(
        Query<TEntity>? navigation = null,
        CancellationToken cancellationToken = default);

    /// <inheritdoc cref="DbContext.AddAsync{TEntity}(TEntity, CancellationToken)" />
    Task<TEntity> AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default);

    /// <inheritdoc cref="DbContext.Update{TEntity}(TEntity)" />
    Task<TEntity> UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default);

    /// <inheritdoc cref="DbContext.Remove{TEntity}(TEntity)" />
    Task RemoveAsync(
        TEntity entity,
        CancellationToken cancellationToken = default);
}

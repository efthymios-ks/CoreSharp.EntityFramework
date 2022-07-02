using CoreSharp.EntityFramework.Delegates;
using CoreSharp.EntityFramework.Entities.Common;
using CoreSharp.EntityFramework.Entities.Interfaces;
using CoreSharp.EntityFramework.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface used for transactional querying.
    /// Used with <see cref="IUnitOfWork"/> for commiting transactions.
    /// Suggested implementation base with <see cref="RepositoryBase{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">Suggested implementation base with <see cref="EntityBase{TKey}"/>.</typeparam>
    public interface IRepository<TEntity>
        where TEntity : class, IEntity
    {
        //Methods
        /// <summary>
        /// Get single entity by given key.
        /// </summary>
        /// <param name="key">Primary key to match.</param>
        /// <param name="navigation">Optional argument to build query.</param>
        Task<TEntity> GetAsync(
            object key,
            Query<TEntity> navigation = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all entities.
        /// </summary>
        /// <param name="navigation">Optional argument to build query.</param>
        Task<IEnumerable<TEntity>> GetAsync(
            Query<TEntity> navigation = null,
            CancellationToken cancellationToken = default);

        /// <inheritdoc cref="DbContext.AddAsync{TEntity}(TEntity, CancellationToken)" />
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <inheritdoc cref="DbContext.Update{TEntity}(TEntity)" />
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <inheritdoc cref="DbContext.Remove{TEntity}(TEntity)" />
        Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);
    }
}

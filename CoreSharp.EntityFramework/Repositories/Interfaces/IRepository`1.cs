using CoreSharp.EntityFramework.Models.Abstracts;
using CoreSharp.EntityFramework.Models.Interfaces;
using CoreSharp.EntityFramework.Repositories.Abstracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Repositories.Interfaces
{
    /// <summary>
    /// Suggested implementation with <see cref="RepositoryBase{TKey}"/>.
    /// </summary>
    /// <typeparam name="TEntity">Suggested implementation with <see cref="EntityBase{TKey}"/>.</typeparam>
    public interface IRepository<TEntity> where TEntity : class, IEntity
    {
        //Methods
        /// <summary>
        /// Get single entity by given key.
        /// </summary>
        /// <param name="key">Value to match.</param>
        /// <param name="navigation">Optional argument to build query.</param> 
        Task<TEntity> GetAsync(
            object key,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> navigation = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all entities.
        /// </summary>
        /// <param name="navigation">Optional argument to build query.</param> 
        Task<IEnumerable<TEntity>> GetAsync(
            Func<IQueryable<TEntity>, IQueryable<TEntity>> navigation = null,
            CancellationToken cancellationToken = default);

        /// <inheritdoc cref="DbContext.AddAsync{TEntity}(TEntity, CancellationToken)" />
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <inheritdoc cref="DbContext.Update{TEntity}(TEntity)" />
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <inheritdoc cref="DbContext.Remove{TEntity}(TEntity)" />
        Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);
    }
}

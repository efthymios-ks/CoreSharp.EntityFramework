using CoreSharp.EntityFramework.Delegates;
using CoreSharp.EntityFramework.Models.Interfaces;
using CoreSharp.Models.Pages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Repositories.Interfaces
{
    /// <inheritdoc />
    public interface IExtendedRepository<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {
        //Methods 
        /// <inheritdoc cref="IRepository{TEntity}.AddAsync(TEntity, CancellationToken)" />
        Task<IEnumerable<TEntity>> AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        /// <inheritdoc cref="IRepository{TEntity}.UpdateAsync(TEntity, CancellationToken)" />
        Task<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        /// <inheritdoc cref="IRepository{TEntity}.RemoveAsync(TEntity, CancellationToken)" />
        Task RemoveAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        /// <inheritdoc cref="IRepository{TEntity}.RemoveAsync(TEntity, CancellationToken)" />
        Task RemoveByKeyAsync(object key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Check any entity exists with given id.
        /// </summary>
        Task<bool> ExistsAsync(object key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Check if the subquery returns one or more records.
        /// </summary>
        Task<bool> ExistsAsync(Query<TEntity> navigation = null, CancellationToken cancellationToken = default);

        /// <inheritdoc cref="EntityFrameworkQueryableExtensions.CountAsync{TSource}(IQueryable{TSource}, CancellationToken)" />
        Task<long> CountAsync(Query<TEntity> navigation = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Return paged entity collection.
        /// </summary>
        Task<Page<TEntity>> GetPageAsync(int pageNumber, int pageSize, Query<TEntity> navigation = null, CancellationToken cancellationToken = default);
    }
}

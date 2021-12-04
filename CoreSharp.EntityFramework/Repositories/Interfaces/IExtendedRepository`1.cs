using CoreSharp.EntityFramework.Models.Interfaces;
using System.Collections.Generic;
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
    }
}

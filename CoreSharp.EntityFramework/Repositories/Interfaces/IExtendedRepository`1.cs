using CoreSharp.EntityFramework.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
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
        /// <inheritdoc cref="DbContext.AddAsync{TEntity}(TEntity, CancellationToken)" />
        Task<IEnumerable<TEntity>> AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        /// <inheritdoc cref="DbContext.Update{TEntity}(TEntity)" />
        Task<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        /// <inheritdoc cref="DbContext.Remove{TEntity}(TEntity)" />
        Task RemoveAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    }
}

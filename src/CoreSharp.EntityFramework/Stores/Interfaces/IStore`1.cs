using CoreSharp.EntityFramework.Entities.Common;
using CoreSharp.EntityFramework.Entities.Interfaces;
using CoreSharp.EntityFramework.Repositories.Interfaces;
using CoreSharp.EntityFramework.Stores.Common;

namespace CoreSharp.EntityFramework.Stores.Interfaces
{
    /// <summary>
    /// Store interface used for single entity querying.
    /// All queries are executed when called without the
    /// need of commiting, meaning that transactions are
    /// not supported.
    /// Suggested implementation base with <see cref="StoreBase{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">Suggested implementation base with <see cref="EntityBase{TKey}"/>.</typeparam>
    public interface IStore<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {
    }
}

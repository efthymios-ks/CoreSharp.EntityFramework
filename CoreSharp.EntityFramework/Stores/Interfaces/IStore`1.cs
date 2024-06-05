using CoreSharp.EntityFramework.Entities.Abstracts;
using CoreSharp.EntityFramework.Entities.Interfaces;
using CoreSharp.EntityFramework.Repositories.Interfaces;
using CoreSharp.EntityFramework.Stores.Abstracts;

namespace CoreSharp.EntityFramework.Stores.Interfaces;

/// <summary>
/// Store interface used for single entity querying.
/// All queries are executed when called without the
/// need of commiting, meaning that transactions are
/// not supported.
/// Suggested implementation base with <see cref="StoreBase{TEntity, TKey}"/>.
/// </summary>
/// <typeparam name="TEntity">Suggested implementation base with <see cref="EntityBase{TKey}"/>.</typeparam>
/// <typeparam name="TKey">PK type of TEntity.</typeparam>
public interface IStore<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
}

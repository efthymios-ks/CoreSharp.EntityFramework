using CoreSharp.EntityFramework.Entities.Interfaces;
using CoreSharp.EntityFramework.Repositories.Interfaces;

namespace CoreSharp.EntityFramework.Stores.Interfaces;

/// <inheritdoc cref="IStore{TEntity, TKey}" />
public interface IExtendedStore<TEntity, TKey> : IExtendedRepository<TEntity, TKey>, IStore<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
}

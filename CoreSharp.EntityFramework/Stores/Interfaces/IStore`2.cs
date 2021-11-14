using CoreSharp.EntityFramework.Models.Interfaces;
using CoreSharp.EntityFramework.Repositories.Interfaces;

namespace CoreSharp.EntityFramework.Stores.Interfaces
{
    /// <inheritdoc />
    public interface IStore<TEntity, TKey> : IStore<TEntity>, IRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {
    }
}

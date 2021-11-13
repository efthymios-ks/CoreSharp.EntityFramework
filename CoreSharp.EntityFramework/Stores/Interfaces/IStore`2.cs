using CoreSharp.EntityFramework.Models.Interfaces;

namespace CoreSharp.EntityFramework.Stores.Interfaces
{
    /// <inheritdoc />
    public interface IStore<TEntity, TKey> : IStore<TEntity>
        where TEntity : class, IEntity<TKey>
    {
    }
}

using CoreSharp.EntityFramework.Models.Interfaces;

namespace CoreSharp.EntityFramework.Store.Interfaces
{
    public interface IStore<TEntity, TKey> : IStore<TEntity>
        where TEntity : class, IEntity<TKey>
    {
    }
}

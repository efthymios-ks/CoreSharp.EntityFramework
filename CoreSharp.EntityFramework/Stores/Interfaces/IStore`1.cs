using CoreSharp.EntityFramework.Models.Interfaces;
using CoreSharp.EntityFramework.Repositories.Interfaces;

namespace CoreSharp.EntityFramework.Stores.Interfaces
{
    /// <summary>
    /// Store interface used for single entity querying.
    /// All queries are executed when called without the
    /// need of commiting, meaning that transactions are
    /// not supported.
    /// </summary>
    public interface IStore<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {
    }
}

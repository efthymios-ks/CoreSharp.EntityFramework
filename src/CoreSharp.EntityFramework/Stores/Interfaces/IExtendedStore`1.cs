using CoreSharp.EntityFramework.Models.Interfaces;
using CoreSharp.EntityFramework.Repositories.Interfaces;

namespace CoreSharp.EntityFramework.Stores.Interfaces
{
    /// <inheritdoc cref="IStore{TEntity}" />
    public interface IExtendedStore<TEntity> : IExtendedRepository<TEntity>, IStore<TEntity>
        where TEntity : class, IEntity
    {
    }
}

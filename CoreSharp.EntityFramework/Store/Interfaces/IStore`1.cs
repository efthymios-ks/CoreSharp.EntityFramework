using CoreSharp.EntityFramework.Models.Interfaces;
using CoreSharp.EntityFramework.Repositories.Interfaces;

namespace CoreSharp.EntityFramework.Store.Interfaces
{
    public interface IStore<TEntity> : IRepository<TEntity>, IUnitOfWork
        where TEntity : class, IEntity
    {
    }
}

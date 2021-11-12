using CoreSharp.EntityFramework.Models.Interfaces;
using CoreSharp.EntityFramework.Repositories.Interfaces;
using System;

namespace CoreSharp.EntityFramework.Store.Interfaces
{
    public interface IStore<TEntity> : IRepository<TEntity>, IDisposable
        where TEntity : class, IEntity
    {
    }
}

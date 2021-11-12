using CoreSharp.EntityFramework.Models.Interfaces;
using CoreSharp.EntityFramework.Store.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Store.Abstracts
{
    public abstract class StoreBase<TEntity, TKey> : StoreBase<TEntity>, IStore<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {
        //Constructors
        protected StoreBase(DbContext dbContext) : base(dbContext)
        {
        }
    }
}

using CoreSharp.EntityFramework.Models.Interfaces;
using CoreSharp.EntityFramework.Stores.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoreSharp.EntityFramework.Stores.Abstracts
{
    /// <inheritdoc />
    public abstract class StoreBase<TEntity, TKey> : StoreBase<TEntity>, IStore<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {
        //Constructors
        protected StoreBase(DbContext dbContext) : base(dbContext)
        {
        }
    }
}

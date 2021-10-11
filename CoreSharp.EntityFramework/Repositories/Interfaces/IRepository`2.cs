using CoreSharp.EntityFramework.Models.Abstracts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Repositories.Interfaces
{
    /// <inheritdoc />
    public interface IRepository<TEntity, TKey> : IRepository<TEntity> where TEntity : BaseEntity<TKey>
    {
        //Methods 
        /// <inheritdoc cref="IRepository{TEntity}.GetAsync(object, Func{IQueryable{TEntity}, IQueryable{TEntity}})"/>
        Task<TEntity> GetAsync(TKey key, Func<IQueryable<TEntity>, IQueryable<TEntity>> navigation = null);
    }
}

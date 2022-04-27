using System.Linq;

namespace CoreSharp.EntityFramework.Delegates
{
    /// <summary>
    /// Query navigation.
    /// </summary>
    public delegate IQueryable<TEntity> Query<TEntity>(IQueryable<TEntity> query);
}

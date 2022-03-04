using CoreSharp.EntityFramework.Delegates;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Queries.Abstract
{
    public abstract class RepositoryNavigationBase<TEntity>
    {
        //Properties
        public Query<TEntity> Navigation { get; init; }
    }
}

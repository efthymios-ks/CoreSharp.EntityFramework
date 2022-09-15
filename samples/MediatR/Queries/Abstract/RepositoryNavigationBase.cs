using CoreSharp.EntityFramework.Delegates;

namespace MediatR.Queries.Abstract;

public abstract class RepositoryNavigationBase<TEntity>
{
    // Properties
    public Query<TEntity> Navigation { get; init; }
}

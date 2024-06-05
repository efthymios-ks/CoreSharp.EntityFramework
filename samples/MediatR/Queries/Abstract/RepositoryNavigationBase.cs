using CoreSharp.EntityFramework.Delegates;

namespace MediatR.Queries.Abstract;

public abstract record RepositoryNavigationBase<TEntity>(Query<TEntity>? Navigation = null);

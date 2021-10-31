using System;
using System.Linq;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Queries.Abstract
{
    public abstract class RepositoryNavigationBase<TEntity>
    {
        //Properties
        public Func<IQueryable<TEntity>, IQueryable<TEntity>> Navigation { get; init; }
    }
}

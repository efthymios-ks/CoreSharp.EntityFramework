using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models;
using CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Queries.Abstract;
using MediatR;
using System.Collections.Generic;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Queries
{
    public class GetTeachersQuery : RepositoryNavigationBase<Teacher>, IRequest<IEnumerable<Teacher>>
    {
    }
}

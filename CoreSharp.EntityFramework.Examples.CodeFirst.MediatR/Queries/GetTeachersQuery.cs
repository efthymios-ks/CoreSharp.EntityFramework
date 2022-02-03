using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.UnitOfWork.Interfaces;
using CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Queries.Abstract;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Queries
{
    public class GetTeachersQuery : RepositoryNavigationBase<Teacher>, IRequest<IEnumerable<Teacher>>
    {
    }

    public class GetTeachersQueryHandler : IRequestHandler<GetTeachersQuery, IEnumerable<Teacher>>
    {
        //Fields
        private readonly IAppUnitOfWork _appUnitOfWork;

        //Constructors
        public GetTeachersQueryHandler(IAppUnitOfWork appUnitOfWork)
            => _appUnitOfWork = appUnitOfWork;

        //Methods
        public async Task<IEnumerable<Teacher>> Handle(GetTeachersQuery request, CancellationToken cancellationToken)
            => await _appUnitOfWork.Teachers.GetAsync(navigation: request.Navigation, cancellationToken: cancellationToken);
    }
}

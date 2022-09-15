using Domain.Database.Models;
using Domain.Database.UnitOfWorks.Interfaces;
using MediatR.Queries.Abstract;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Queries;

public class GetTeachersQuery : RepositoryNavigationBase<Teacher>, IRequest<IEnumerable<Teacher>>
{
}

public class GetTeachersQueryHandler : IRequestHandler<GetTeachersQuery, IEnumerable<Teacher>>
{
    // Fields
    private readonly IAppUnitOfWork _appUnitOfWork;

    // Constructors
    public GetTeachersQueryHandler(IAppUnitOfWork appUnitOfWork)
        => _appUnitOfWork = appUnitOfWork;

    // Methods
    public async Task<IEnumerable<Teacher>> Handle(GetTeachersQuery request, CancellationToken cancellationToken)
        => await _appUnitOfWork.Teachers.GetAsync(navigation: request.Navigation, cancellationToken: cancellationToken);
}

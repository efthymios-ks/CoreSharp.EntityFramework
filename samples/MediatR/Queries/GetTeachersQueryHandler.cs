using Domain.Database.Models;
using Domain.Database.UnitOfWorks.Interfaces;

namespace MediatR.Queries;

public sealed class GetTeachersQueryHandler(IAppUnitOfWork appUnitOfWork) : IRequestHandler<GetTeachersQuery, IEnumerable<Teacher>>
{
    // Fields
    private readonly IAppUnitOfWork _appUnitOfWork = appUnitOfWork;

    // Methods
    public Task<IEnumerable<Teacher>> Handle(GetTeachersQuery request, CancellationToken cancellationToken)
        => _appUnitOfWork.Teachers.GetAsync(navigation: request.Navigation, cancellationToken: cancellationToken);
}

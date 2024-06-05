using Domain.Database.Models;
using Domain.Database.UnitOfWorks.Interfaces;

namespace MediatR.Queries;

public sealed class GetTeacherByIdQueryHandler(IAppUnitOfWork appUnitOfWork) : IRequestHandler<GetTeacherByIdQuery, Teacher?>
{
    // Fields
    private readonly IAppUnitOfWork _appUnitOfWork = appUnitOfWork;

    // Methods
    public Task<Teacher?> Handle(GetTeacherByIdQuery request, CancellationToken cancellationToken)
        => _appUnitOfWork.Teachers.GetAsync(request.TeacherId, request.Navigation, cancellationToken);
}

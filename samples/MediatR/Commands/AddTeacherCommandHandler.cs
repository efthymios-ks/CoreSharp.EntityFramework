using Domain.Database.Models;
using Domain.Database.UnitOfWorks.Interfaces;

namespace MediatR.Commands;

public sealed class AddTeacherCommandHandler(IAppUnitOfWork appUnitOfWork) : IRequestHandler<AddTeacherCommand, Teacher>
{
    // Fields
    private readonly IAppUnitOfWork _appUnitOfWork = appUnitOfWork;

    // Methods
    public async Task<Teacher> Handle(AddTeacherCommand request, CancellationToken cancellationToken)
    {
        _ = request.Teacher ?? throw new InvalidOperationException($"{nameof(request.Teacher)} cannot be null.");

        var createdTeacher = await _appUnitOfWork.Teachers.AddAsync(request.Teacher, cancellationToken);
        await _appUnitOfWork.CommitAsync(cancellationToken);
        return createdTeacher;
    }
}

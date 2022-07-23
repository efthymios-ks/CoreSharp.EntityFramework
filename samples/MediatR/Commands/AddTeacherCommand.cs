using Domain.Database.Models;
using Domain.Database.UnitOfWorks.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Commands;

public class AddTeacherCommand : IRequest<Teacher>
{
    //Constructors
    public AddTeacherCommand(Teacher teacher)
        => Teacher = teacher ?? throw new ArgumentNullException(nameof(teacher));

    //Properties
    public Teacher Teacher { get; }
}

public class AddTeacherCommandHandler : IRequestHandler<AddTeacherCommand, Teacher>
{
    //Fields
    private readonly IAppUnitOfWork _appUnitOfWork;

    //Constructors
    public AddTeacherCommandHandler(IAppUnitOfWork appUnitOfWork)
        => _appUnitOfWork = appUnitOfWork;

    //Methods
    public async Task<Teacher> Handle(AddTeacherCommand request, CancellationToken cancellationToken)
    {
        _ = request.Teacher ?? throw new ArgumentNullException(nameof(request), $"{nameof(request.Teacher)} cannot be null.");

        var createdTeacher = await _appUnitOfWork.Teachers.AddAsync(request.Teacher, cancellationToken);
        await _appUnitOfWork.CommitAsync(cancellationToken);
        return createdTeacher;
    }
}

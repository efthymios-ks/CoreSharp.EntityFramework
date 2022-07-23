using Domain.Database.Models;
using Domain.Database.Stores.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Commands;

public class UpdateTeacherCommand : IRequest<Teacher>
{
    //Constructors
    public UpdateTeacherCommand(Teacher teacher)
        => Teacher = teacher ?? throw new ArgumentNullException(nameof(teacher));

    //Properties
    public Teacher Teacher { get; }
}

public class UpdateTeacherCommandHandler : IRequestHandler<UpdateTeacherCommand, Teacher>
{
    //Fields
    private readonly ITeacherStore _teacherStore;

    //Constructors
    public UpdateTeacherCommandHandler(ITeacherStore teacherStore)
        => _teacherStore = teacherStore;

    //Methods
    public async Task<Teacher> Handle(UpdateTeacherCommand request, CancellationToken cancellationToken)
    {
        _ = request.Teacher ?? throw new ArgumentException($"{nameof(request.Teacher)} cannot be null.", nameof(request));

        return await _teacherStore.UpdateAsync(request.Teacher, cancellationToken);
    }
}

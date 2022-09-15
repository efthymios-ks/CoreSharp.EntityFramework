using Domain.Database.Models;
using Domain.Database.UnitOfWorks.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Commands;

public class RemoveTeacherCoursesCommand : IRequest<Teacher>
{
    // Constructors
    public RemoveTeacherCoursesCommand(Guid teacherId)
        => TeacherId = teacherId;

    // Properties
    public Guid TeacherId { get; }
}

public class RemoveTeacherCoursesCommandHandler : IRequestHandler<RemoveTeacherCoursesCommand, Teacher>
{
    // Fields 
    private readonly IAppUnitOfWork _appUnitOfWork;

    // Constructors
    public RemoveTeacherCoursesCommandHandler(IAppUnitOfWork appUnitOfWork)
        => _appUnitOfWork = appUnitOfWork;

    // Methods
    public async Task<Teacher> Handle(RemoveTeacherCoursesCommand request, CancellationToken cancellationToken)
    {
        var teacher = await _appUnitOfWork.Teachers.GetAsync(request.TeacherId, q => q.Include(t => t.Courses), cancellationToken);
        _ = teacher ?? throw new ArgumentOutOfRangeException($"{nameof(Teacher)} with {nameof(Teacher.Id)}=`{request.TeacherId}` not found.");

        if (teacher.Courses.Count == 0)
            return teacher;

        foreach (var course in teacher.Courses)
            await _appUnitOfWork.Courses.RemoveAsync(course, cancellationToken);
        teacher.Courses.Clear();
        await _appUnitOfWork.Teachers.UpdateAsync(teacher, cancellationToken);
        await _appUnitOfWork.CommitAsync(cancellationToken);
        return teacher;
    }
}

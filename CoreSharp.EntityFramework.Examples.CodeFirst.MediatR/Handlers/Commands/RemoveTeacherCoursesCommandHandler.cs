using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.UnitOfWork.Interfaces;
using CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Handlers.Commands
{
    public class RemoveTeacherCoursesCommandHandler : IRequestHandler<RemoveTeacherCoursesCommand, Teacher>
    {
        //Fields 
        private readonly ISchoolUnitOfWork _schoolUnitOfWork;

        //Constructors
        public RemoveTeacherCoursesCommandHandler(ISchoolUnitOfWork schoolUnitOfWork)
            => _schoolUnitOfWork = schoolUnitOfWork;

        //Methods
        public async Task<Teacher> Handle(RemoveTeacherCoursesCommand request, CancellationToken cancellationToken)
        {
            var teacher = await _schoolUnitOfWork.Teachers.GetAsync(request.TeacherId, q => q.Include(t => t.Courses), cancellationToken);
            if (teacher is null)
                throw new ArgumentOutOfRangeException($"{nameof(Teacher)} with {nameof(Teacher.Id)}=`{request.TeacherId}` not found.");

            if (!teacher.Courses.Any())
                return teacher;

            foreach (var course in teacher.Courses)
                await _schoolUnitOfWork.Courses.RemoveAsync(course, cancellationToken);
            teacher.Courses.Clear();
            await _schoolUnitOfWork.Teachers.UpdateAsync(teacher, cancellationToken);
            await _schoolUnitOfWork.CommitAsync(cancellationToken);
            return teacher;
        }
    }
}

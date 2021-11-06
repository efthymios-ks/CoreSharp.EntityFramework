using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Repositories.Interfaces;
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
        private readonly SchoolDbContext _schoolDbContext;
        private readonly ITeacherRepository _teacherRepository;
        private readonly ICourseRepository _courseRepository;

        //Constructors
        public RemoveTeacherCoursesCommandHandler(SchoolDbContext schoolDbContext,
            ITeacherRepository teacherRepository,
            ICourseRepository courseRepository)
        {
            _schoolDbContext = schoolDbContext ?? throw new ArgumentNullException(nameof(schoolDbContext));
            _teacherRepository = teacherRepository ?? throw new ArgumentNullException(nameof(teacherRepository));
            _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
        }

        //Methods
        public async Task<Teacher> Handle(RemoveTeacherCoursesCommand request, CancellationToken cancellationToken)
        {
            var teacher = await _teacherRepository.GetAsync(request.TeacherId, q => q.Include(t => t.Courses), cancellationToken);
            if (teacher is null)
                throw new ArgumentOutOfRangeException($"{nameof(Teacher)} with {nameof(Teacher.Id)}=`{request.TeacherId}` not found.");

            if (teacher.Courses.Any())
            {
                foreach (var course in teacher.Courses)
                    await _courseRepository.RemoveAsync(course, cancellationToken);
                teacher.Courses.Clear();
                await _teacherRepository.UpdateAsync(teacher, cancellationToken);
                await _schoolDbContext.SaveChangesAsync(cancellationToken);
            }
            return teacher;
        }
    }
}

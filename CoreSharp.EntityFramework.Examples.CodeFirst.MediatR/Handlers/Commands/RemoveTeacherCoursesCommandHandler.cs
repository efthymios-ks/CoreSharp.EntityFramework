using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Repositories.Interfaces;
using CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Commands;
using MediatR;
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
            _ = request.Teacher ?? throw new ArgumentNullException(nameof(request));

            var teacher = request.Teacher;
            if (teacher.Courses.Any())
            {
                foreach (var course in teacher.Courses)
                    await _courseRepository.RemoveAsync(course);
                teacher.Courses.Clear();
                await _teacherRepository.UpdateAsync(teacher);
                await _schoolDbContext.SaveChangesAsync();
            }
            return teacher;
        }
    }
}

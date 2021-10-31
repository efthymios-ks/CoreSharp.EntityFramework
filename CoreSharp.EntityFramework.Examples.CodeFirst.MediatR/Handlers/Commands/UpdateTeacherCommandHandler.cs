using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Repositories.Interfaces;
using CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Commands;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Handlers.Commands
{
    public class UpdateTeacherCommandHandler : IRequestHandler<UpdateTeacherCommand, Teacher>
    {
        //Fields
        private readonly SchoolDbContext _schoolDbContext;
        private readonly ITeacherRepository _teacherRepository;

        //Constructors
        public UpdateTeacherCommandHandler(SchoolDbContext schoolDbContext,
            ITeacherRepository teacherRepository)
        {
            _schoolDbContext = schoolDbContext ?? throw new ArgumentNullException(nameof(schoolDbContext));
            _teacherRepository = teacherRepository ?? throw new ArgumentNullException(nameof(teacherRepository));
        }

        //Methods
        public async Task<Teacher> Handle(UpdateTeacherCommand request, CancellationToken cancellationToken)
        {
            _ = request.Teacher ?? throw new ArgumentNullException(nameof(request.Teacher));

            var teacher = await _teacherRepository.UpdateAsync(request.Teacher);
            await _schoolDbContext.SaveChangesAsync();
            return teacher;
        }
    }
}

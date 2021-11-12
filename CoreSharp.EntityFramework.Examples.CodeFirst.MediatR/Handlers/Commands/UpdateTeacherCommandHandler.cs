using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.UnitOfWork.Interfaces;
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
        private readonly ISchoolUnitOfWork _schoolUnitOfWork;

        //Constructors
        public UpdateTeacherCommandHandler(ISchoolUnitOfWork schoolUnitOfWork)
        {
            _schoolUnitOfWork = schoolUnitOfWork ?? throw new ArgumentNullException(nameof(schoolUnitOfWork));
        }

        //Methods
        public async Task<Teacher> Handle(UpdateTeacherCommand request, CancellationToken cancellationToken)
        {
            _ = request.Teacher ?? throw new NullReferenceException($"{nameof(Teacher)} cannot be null.");

            var teacher = await _schoolUnitOfWork.Teachers.UpdateAsync(request.Teacher, cancellationToken);
            await _schoolUnitOfWork.CommitAsync(cancellationToken);
            return teacher;
        }
    }
}

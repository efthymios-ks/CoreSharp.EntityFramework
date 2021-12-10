using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.UnitOfWork.Interfaces;
using CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Commands;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Handlers.Commands
{
    public class AddTeacherCommandHandler : IRequestHandler<AddTeacherCommand, Teacher>
    {
        //Fields
        private readonly ISchoolUnitOfWork _schoolUnitOfWork;

        //Constructors
        public AddTeacherCommandHandler(ISchoolUnitOfWork schoolUnitOfWork)
            => _schoolUnitOfWork = schoolUnitOfWork;

        //Methods
        public async Task<Teacher> Handle(AddTeacherCommand request, CancellationToken cancellationToken)
        {
            _ = request.Teacher ?? throw new NullReferenceException($"{nameof(request.Teacher)} cannot be null.");

            var createdTeacher = await _schoolUnitOfWork.Teachers.AddAsync(request.Teacher, cancellationToken);
            await _schoolUnitOfWork.CommitAsync(cancellationToken);
            return createdTeacher;
        }
    }
}

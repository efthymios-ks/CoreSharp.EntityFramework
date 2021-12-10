using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models;
using MediatR;
using System;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Commands
{
    public class RemoveTeacherCoursesCommand : IRequest<Teacher>
    {
        //Constructors
        public RemoveTeacherCoursesCommand(Guid teacherId)
            => TeacherId = teacherId;

        //Properties
        public Guid TeacherId { get; }
    }
}

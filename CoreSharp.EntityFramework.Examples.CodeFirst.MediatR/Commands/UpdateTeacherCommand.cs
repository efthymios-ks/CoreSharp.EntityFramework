using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models;
using MediatR;
using System;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Commands
{
    public class UpdateTeacherCommand : IRequest<Teacher>
    {
        //Constructors
        public UpdateTeacherCommand(Teacher teacher)
        {
            Teacher = teacher ?? throw new ArgumentNullException(nameof(teacher));
        }

        //Properties
        public Teacher Teacher { get; }
    }
}

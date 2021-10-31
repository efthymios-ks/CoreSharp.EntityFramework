using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models;
using MediatR;
using System;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Commands
{
    public class AddTeacherCommand : IRequest<Teacher>
    {
        //Constructors
        public AddTeacherCommand(Teacher teacher)
        {
            Teacher = teacher ?? throw new ArgumentNullException(nameof(teacher));
        }

        //Properties
        public Teacher Teacher { get; }
    }
}

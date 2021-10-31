using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models;
using MediatR;
using System;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Commands
{
    public class RemoveTeacherCoursesCommand : IRequest<Teacher>
    {
        //Constructors
        public RemoveTeacherCoursesCommand(Teacher teacher)
        {
            Teacher = teacher ?? throw new ArgumentNullException(nameof(teacher));
        }

        //Properties
        public Teacher Teacher { get; }
    }
}

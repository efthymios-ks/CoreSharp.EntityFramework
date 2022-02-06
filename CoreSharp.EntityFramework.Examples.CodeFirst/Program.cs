using CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Commands;
using CoreSharp.EntityFramework.Examples.CodeFirst.MediatR.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Examples.CodeFirst
{
    internal static class Program
    {
        //Methods 
        private static async Task Main()
        {
            var services = Startup.ConfigureServices();
            var mediatR = services.GetRequiredService<IMediator>();

            //Get first teacher
            var query = new GetTeachersQuery
            {
                Navigation = q => q.Include(t => t.Courses)
            };
            var teacher = (await mediatR.Send(query)).FirstOrDefault();

            //Create one, if none 
            if (teacher is null)
            {
                var command = new AddTeacherCommand(new()
                {
                    Name = "Efthymios"
                });
                teacher = await mediatR.Send(command);
            }

            //Remove courses 
            if (teacher.Courses.Any())
            {
                var command = new RemoveTeacherCoursesCommand(teacher.Id);
                teacher = await mediatR.Send(command);
            }

            //Add course from scratch
            {
                teacher.Courses.Add(new()
                {
                    Name = "Math"
                });
                var command = new UpdateTeacherCommand(teacher);
                teacher = await mediatR.Send(command);
            }

            Console.ReadLine();
        }
    }
}

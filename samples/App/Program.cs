﻿using CoreSharp.EntityFramework.Samples.MediatR.Commands;
using CoreSharp.EntityFramework.Samples.MediatR.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Samples.App;

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
                Name = "Efthymios Koktsidis"
            });
            teacher = await mediatR.Send(command);
        }

        //Update teacher 
        {
            teacher.Name = $"Efthymios ({DateTime.Now.Millisecond})";
            var command = new UpdateTeacherCommand(teacher);
            teacher = await mediatR.Send(command);
        }

        //Remove courses 
        if (teacher.Courses.Count > 0)
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

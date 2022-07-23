using Domain.Database.Models;
using MediatR;
using MediatR.Commands;
using MediatR.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace App;

internal static class Program
{
    //Methods 
    private static async Task Main()
    {
        var services = Startup.ConfigureServices();
        var mediatR = services.GetRequiredService<IMediator>();

        var teacher = await GetFirstTeacherAsync(mediatR);
        if (teacher is null)
            teacher = await CreateTeacherAsync(mediatR);
        teacher = await UpdateTeacherAsync(mediatR, teacher);
        teacher = await RemoveTeacherCoursesAsync(mediatR, teacher);
        await AddTeacherCoursesAsync(mediatR, teacher);

        Console.ReadLine();
    }

    private static async Task<Teacher> GetFirstTeacherAsync(IMediator mediatR)
    {
        var query = new GetTeachersQuery
        {
            Navigation = q => q.Include(t => t.Courses)
        };
        return (await mediatR.Send(query)).FirstOrDefault();
    }

    private static async Task<Teacher> CreateTeacherAsync(IMediator mediatR)
    {
        var command = new AddTeacherCommand(new()
        {
            Name = "Efthymios Koktsidis"
        });
        return await mediatR.Send(command);
    }

    private static async Task<Teacher> UpdateTeacherAsync(IMediator mediatR, Teacher teacher)
    {
        teacher.Name = $"Efthymios ({DateTime.Now.Millisecond})";
        var command = new UpdateTeacherCommand(teacher);
        return await mediatR.Send(command);
    }

    private static async Task<Teacher> RemoveTeacherCoursesAsync(IMediator mediatR, Teacher teacher)
    {
        if (teacher.Courses.Count == 0)
            return teacher;

        var command = new RemoveTeacherCoursesCommand(teacher.Id);
        return await mediatR.Send(command);
    }

    private static async Task<Teacher> AddTeacherCoursesAsync(IMediator mediatR, Teacher teacher)
    {
        teacher.Courses.Add(new()
        {
            Name = "Math"
        });
        var command = new UpdateTeacherCommand(teacher);
        return await mediatR.Send(command);
    }
}

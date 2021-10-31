﻿using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Models;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.Repositories.Interfaces;
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

            var dbContext = services.GetRequiredService<DbContext>();
            var teacherRepository = services.GetRequiredService<ITeacherRepository>();
            var courseRepository = services.GetRequiredService<ICourseRepository>();

            //Get first teacher
            var teacher = (await teacherRepository.GetAsync(navigation: q => q
                                                            .Take(1)
                                                            .Include(t => t.Courses)))
                                                   .FirstOrDefault();

            //Create one, if none 
            if (teacher is null)
            {
                var newTeacher = new Teacher()
                {
                    Name = "Efthymios"
                };
                teacher = await teacherRepository.AddAsync(newTeacher);
                await dbContext.SaveChangesAsync();
            }

            //Remove courses 
            if (teacher.Courses.Any())
            {
                foreach (var course in teacher.Courses)
                    await courseRepository.RemoveAsync(course);

                teacher.Courses.Clear();
                await teacherRepository.UpdateAsync(teacher);
                await dbContext.SaveChangesAsync();
            }

            //Add course from scratch
            {
                var course = new Course()
                {
                    Name = "Math"
                };
                teacher.Courses.Add(course);
                await teacherRepository.UpdateAsync(teacher);
                await dbContext.SaveChangesAsync();
            }

            Console.ReadLine();
        }
    }
}

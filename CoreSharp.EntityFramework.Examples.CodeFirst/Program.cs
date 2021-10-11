using CoreSharp.EntityFramework.Examples.CodeFirst.Database;
using CoreSharp.EntityFramework.Examples.CodeFirst.Database.Models;
using CoreSharp.EntityFramework.Examples.CodeFirst.Database.Repositories;
using CoreSharp.EntityFramework.Examples.CodeFirst.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;

namespace CoreSharp.EntityFramework.Examples.CodeFirst
{
    internal static class Program
    {
        //Methods 
        private static async Task Main()
        {
            var services = GetServiceProvider();

            var dbContext = services.GetRequiredService<DbContext>();
            var teacherRepository = services.GetRequiredService<ITeacherRepository>();

            var teachers = await teacherRepository.GetAsync(navigation: q => q.Include(t => t.Courses));
            var teacher = teachers.FirstOrDefault();
            if (teacher is not null)
            {
                if (teacher.Courses.Any())
                {
                    teacher.Courses.Clear();
                    await teacherRepository.UpdateAsync(teacher);
                    await dbContext.SaveChangesAsync();
                }

                var course = new Course()
                {
                    Name = "Math"
                };
                teacher.Courses.Add(course);
                await teacherRepository.UpdateAsync(teacher);
            }

            //Save changes 
            await dbContext.SaveChangesAsync();
        }

        private static ServiceProvider GetServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<DbContext, SchoolDbContext>();
            serviceCollection.AddScoped<ITeacherRepository, TeacherRepository>();
            return serviceCollection.BuildServiceProvider();
        }
    }
}

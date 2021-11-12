using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.UnitOfWork;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.UnitOfWork.Interfaces;
using CoreSharp.EntityFramework.Extensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CoreSharp.EntityFramework.Examples.CodeFirst
{
    /// <summary>
    /// Pseudo-startup class.
    /// </summary>
    internal static class Startup
    {
        //Methods 
        public static ServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();

            //1. Add DbContext 
            serviceCollection.AddScoped<SchoolDbContext>();
            //2. Add UnitOfWork 
            serviceCollection.AddScoped<ISchoolUnitOfWork, SchoolUnitOfWork>();
            //3. Add Repositories
            serviceCollection.AddRepositories(typeof(SchoolDbContext).Assembly);
            //4. Optionally, Add IMediatR
            serviceCollection.AddMediatR(typeof(MediatR.AssemblyReferenceHook));

            return serviceCollection.BuildServiceProvider();
        }
    }
}

using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database;
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

            serviceCollection.AddScoped<SchoolDbContext>();
            serviceCollection.AddRepositories(typeof(SchoolDbContext).Assembly);
            serviceCollection.AddMediatR(typeof(MediatR.AssemblyReferenceHook));

            return serviceCollection.BuildServiceProvider();
        }
    }
}

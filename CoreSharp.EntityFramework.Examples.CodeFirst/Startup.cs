using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database;
using CoreSharp.EntityFramework.Examples.CodeFirst.Extensions;
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

            //2a. Add Repositories or...
            serviceCollection.AddAppRepositories();

            //2b. Add Stores  
            serviceCollection.AddAppStores();

            //3. Optionally, Add IMediatR
            serviceCollection.AddMediatR(typeof(MediatR.AssemblyReferenceHook));

            return serviceCollection.BuildServiceProvider();
        }
    }
}

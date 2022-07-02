using CoreSharp.EntityFramework.Samples.App.Extensions;
using CoreSharp.EntityFramework.Samples.Domain.Database;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CoreSharp.EntityFramework.Samples.App;

/// <summary>
/// Pseudo-startup class.
/// </summary>
internal static class Startup
{
    //Methods 
    public static IServiceProvider ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();

        //1. Add DbContext. 
        serviceCollection.AddScoped<AppDbContext>();

        //2a. Add Repositories or...
        serviceCollection.AddAppRepositories();

        //2b. Add Stores. 
        serviceCollection.AddAppStores();

        //3. Optionally, Add MediatR.
        serviceCollection.AddMediatR(typeof(MediatR.AssemblyReferenceHook));

        return serviceCollection.BuildServiceProvider();
    }
}

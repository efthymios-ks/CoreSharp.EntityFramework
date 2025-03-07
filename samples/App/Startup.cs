﻿using App.Extensions;
using Domain.Database;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace App;

/// <summary>
/// Pseudo-startup class.
/// </summary>
internal static class Startup
{
    // Methods 
    public static IServiceProvider ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddAppLogging();

        // 1. Add DbContext. 
        serviceCollection.AddScoped<AppDbContext>();

        // 2a. Add Repositories or...
        serviceCollection.AddAppRepositories();

        // 2b. Add Stores. 
        serviceCollection.AddAppStores();

        // 3. Optionally, Add MediatR.
        serviceCollection.AddMediatR(
            config => config.RegisterServicesFromAssembly(typeof(AssemblyReferenceHook).Assembly));

        return serviceCollection.BuildServiceProvider();
    }
}

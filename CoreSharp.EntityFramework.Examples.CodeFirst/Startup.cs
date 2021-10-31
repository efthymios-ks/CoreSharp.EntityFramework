﻿using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database;
using CoreSharp.EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CoreSharp.EntityFramework.Examples.CodeFirst
{
    /// <summary>
    /// Pseude-startup class.
    /// </summary>
    internal static class Startup
    {
        //Methods 
        public static ServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddScoped<DbContext, SchoolDbContext>();
            serviceCollection.RegisterRepositories(typeof(SchoolDbContext).Assembly);

            return serviceCollection.BuildServiceProvider();
        }
    }
}

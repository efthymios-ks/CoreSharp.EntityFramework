using CoreSharp.EntityFramework.Extensions;
using CoreSharp.EntityFramework.Repositories.Interfaces;
using CoreSharp.EntityFramework.Stores.Interfaces;
using Domain.Database;
using Domain.Database.UnitOfWorks;
using Domain.Database.UnitOfWorks.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace App.Extensions;

/// <summary>
/// <see cref="IServiceCollection"/> extensions.
/// </summary>
internal static class IServiceCollectionExtensions
{
    /// <summary>
    /// Register all <see cref="IRepository{TEntity}"/> and <see cref="IUnitOfWork"/>.
    /// </summary>
    public static IServiceCollection AddAppRepositories(this IServiceCollection serviceCollection)
    {
        ArgumentNullException.ThrowIfNull(serviceCollection);

        serviceCollection.AddRepositories(typeof(AppDbContext).Assembly);
        serviceCollection.AddExtendedRepositories(typeof(AppDbContext).Assembly);
        serviceCollection.AddScoped<IAppUnitOfWork, AppUnitOfWork>();

        return serviceCollection;
    }

    /// <summary>
    /// Register all <see cref="IStore{TEntity}"/>.
    /// </summary>
    public static IServiceCollection AddAppStores(this IServiceCollection serviceCollection)
    {
        ArgumentNullException.ThrowIfNull(serviceCollection);

        serviceCollection.AddStores(typeof(AppDbContext).Assembly);
        serviceCollection.AddExtendedStores(typeof(AppDbContext).Assembly);

        return serviceCollection;
    }

    public static IServiceCollection AddAppLogging(this IServiceCollection servicesCollection)
    {
        servicesCollection.AddLogging(builder => builder.AddDebug());

        return servicesCollection;
    }
}

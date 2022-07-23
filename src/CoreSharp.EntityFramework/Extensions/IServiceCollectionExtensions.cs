using CoreSharp.EntityFramework.Repositories.Interfaces;
using CoreSharp.EntityFramework.Stores.Interfaces;
using CoreSharp.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CoreSharp.EntityFramework.Extensions;

/// <summary>
/// <see cref="IServiceCollection"/> extensions.
/// </summary>
public static class IServiceCollectionExtensions
{
    //Methods 
    #region Repositories
    /// <inheritdoc cref="AddRepositories(IServiceCollection, Assembly[])" />
    public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
        => serviceCollection.AddRepositories(Assembly.GetEntryAssembly());

    /// <inheritdoc cref="AddRepositories(IServiceCollection, Assembly[])" />
    public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection, IEnumerable<Assembly> assemblies)
        => serviceCollection.AddRepositories(assemblies?.ToArray());

    /// <inheritdoc cref="CoreSharp.Extensions.IServiceCollectionExtensions.AddServices(IServiceCollection, Type, Assembly[])" />
    public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection, params Assembly[] assemblies)
        => serviceCollection.AddServices(typeof(IRepository<>), assemblies);
    #endregion

    #region Stores
    /// <inheritdoc cref="AddStores(IServiceCollection, Assembly[])" />
    public static IServiceCollection AddStores(this IServiceCollection serviceCollection)
        => serviceCollection.AddStores(Assembly.GetEntryAssembly());

    /// <inheritdoc cref="AddStores(IServiceCollection, Assembly[])" />
    public static IServiceCollection AddStores(this IServiceCollection serviceCollection, IEnumerable<Assembly> assemblies)
        => serviceCollection.AddStores(assemblies?.ToArray());

    /// <inheritdoc cref="CoreSharp.Extensions.IServiceCollectionExtensions.AddServices(IServiceCollection, Type, Assembly[])" />
    public static IServiceCollection AddStores(this IServiceCollection serviceCollection, params Assembly[] assemblies)
        => serviceCollection.AddServices(typeof(IStore<>), assemblies);
    #endregion

    #region Extended Repositories
    /// <inheritdoc cref="AddExtendedRepositories(IServiceCollection, Assembly[])" />
    public static IServiceCollection AddExtendedRepositories(this IServiceCollection serviceCollection)
        => serviceCollection.AddExtendedRepositories(Assembly.GetEntryAssembly());

    /// <inheritdoc cref="AddExtendedRepositories(IServiceCollection, Assembly[])" />
    public static IServiceCollection AddExtendedRepositories(this IServiceCollection serviceCollection, IEnumerable<Assembly> assemblies)
        => serviceCollection.AddExtendedRepositories(assemblies?.ToArray());

    /// <inheritdoc cref="CoreSharp.Extensions.IServiceCollectionExtensions.AddServices(IServiceCollection, Type, Assembly[])" />
    public static IServiceCollection AddExtendedRepositories(this IServiceCollection serviceCollection, params Assembly[] assemblies)
        => serviceCollection.AddServices(typeof(IExtendedRepository<>), assemblies);
    #endregion

    #region Extended Stores
    /// <inheritdoc cref="AddExtendedStores(IServiceCollection, Assembly[])" />
    public static IServiceCollection AddExtendedStores(this IServiceCollection serviceCollection)
        => serviceCollection.AddExtendedStores(Assembly.GetEntryAssembly());

    /// <inheritdoc cref="AddExtendedStores(IServiceCollection, Assembly[])" />
    public static IServiceCollection AddExtendedStores(this IServiceCollection serviceCollection, IEnumerable<Assembly> assemblies)
        => serviceCollection.AddExtendedStores(assemblies?.ToArray());

    /// <inheritdoc cref="CoreSharp.Extensions.IServiceCollectionExtensions.AddServices(IServiceCollection, Type, Assembly[])" />
    public static IServiceCollection AddExtendedStores(this IServiceCollection serviceCollection, params Assembly[] assemblies)
        => serviceCollection.AddServices(typeof(IExtendedStore<>), assemblies);
    #endregion
}

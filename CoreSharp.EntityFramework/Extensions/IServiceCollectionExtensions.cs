using CoreSharp.DependencyInjection.ByReflection.Extensions;
using CoreSharp.EntityFramework.Repositories.Interfaces;
using CoreSharp.EntityFramework.Stores.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CoreSharp.EntityFramework.Extensions;

/// <summary>
/// <see cref="IServiceCollection"/> extensions.
/// </summary>
public static class IServiceCollectionExtensions
{
    // Methods 
    /// <inheritdoc cref="DependencyInjection.ByReflection.Extensions.IServiceCollectionExtensions.AddServices(IServiceCollection, Assembly[])" />
    public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection, Assembly[] assemblies)
        => serviceCollection.AddServices(typeof(IRepository<,>), assemblies);

    /// <inheritdoc cref="DependencyInjection.ByReflection.Extensions.IServiceCollectionExtensions.AddServices(IServiceCollection, Type, Assembly[])" />
    public static IServiceCollection AddExtendedRepositories(this IServiceCollection serviceCollection, Assembly[] assemblies)
        => serviceCollection.AddServices(typeof(IExtendedRepository<,>), assemblies);

    /// <inheritdoc cref="DependencyInjection.ByReflection.Extensions.IServiceCollectionExtensions.AddServices(IServiceCollection, Type, Assembly[])"/>
    public static IServiceCollection AddStores(this IServiceCollection serviceCollection, Assembly[] assemblies)
        => serviceCollection.AddServices(typeof(IStore<,>), assemblies);

    /// <inheritdoc cref="DependencyInjection.ByReflection.Extensions.IServiceCollectionExtensions.AddServices(IServiceCollection, Type, Assembly[])" />
    public static IServiceCollection AddExtendedStores(this IServiceCollection serviceCollection, Assembly[] assemblies)
        => serviceCollection.AddServices(typeof(IExtendedStore<,>), assemblies);
}

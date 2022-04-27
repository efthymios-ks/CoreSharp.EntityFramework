using CoreSharp.EntityFramework.Repositories.Interfaces;
using CoreSharp.EntityFramework.Stores.Interfaces;
using CoreSharp.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace CoreSharp.EntityFramework.Extensions
{
    /// <summary>
    /// <see cref="IServiceCollection"/> extensions.
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        //Properties 
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static Type DefaultRepositoryInterfaceType
            => typeof(IRepository<>);
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static Type DefaultStoreInterfaceType
            => typeof(IStore<>);
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static Type DefaultExtendedRepositoryInterfaceType
            => typeof(IExtendedRepository<>);
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static Type DefaultExtendedStoreInterfaceType
            => typeof(IExtendedStore<>);

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
            => serviceCollection.AddServices(DefaultRepositoryInterfaceType, assemblies);
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
            => serviceCollection.AddServices(DefaultStoreInterfaceType, assemblies);
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
            => serviceCollection.AddServices(DefaultExtendedRepositoryInterfaceType, assemblies);
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
            => serviceCollection.AddServices(DefaultExtendedStoreInterfaceType, assemblies);
        #endregion
    }
}

﻿using CoreSharp.EntityFramework.Repositories.Interfaces;
using CoreSharp.EntityFramework.Stores.Interfaces;
using CoreSharp.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CoreSharp.EntityFramework.Extensions
{
    /// <summary>
    /// <see cref="IServiceCollection"/> extensions.
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        //Fields
        private const string InterfacePrefix = "I";
        private const string InterfaceGroupRegexExp = "(?<Name>.+)";

        //Properties
        private static Type DefaultRepositoryInterfaceType
            => typeof(IRepository<>);
        private static Type DefaultStoreInterfaceType
            => typeof(IStore<>);
        private static Type DefaultExtendedRepositoryInterfaceType
            => typeof(IExtendedRepository<>);
        private static Type DefaultExtendedStoreInterfaceType
            => typeof(IExtendedStore<>);
        private static string InterfaceContractRegexExp
            => $"^{InterfacePrefix}{InterfaceGroupRegexExp}$";

        //Methods 
        #region Repositories
        /// <inheritdoc cref="AddRepositories(IServiceCollection, Assembly[])" />
        public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
            => serviceCollection.AddRepositories(Assembly.GetEntryAssembly());

        /// <inheritdoc cref="AddRepositories(IServiceCollection, Assembly[])" />
        public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection, IEnumerable<Assembly> assemblies)
            => serviceCollection.AddRepositories(assemblies?.ToArray());

        /// <inheritdoc cref="AddInterfaces(IServiceCollection, Type, Assembly[])" />
        public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection, params Assembly[] assemblies)
            => serviceCollection.AddInterfaces(DefaultRepositoryInterfaceType, assemblies);
        #endregion

        #region Stores
        /// <inheritdoc cref="AddStores(IServiceCollection, Assembly[])" />
        public static IServiceCollection AddStores(this IServiceCollection serviceCollection)
            => serviceCollection.AddStores(Assembly.GetEntryAssembly());

        /// <inheritdoc cref="AddStores(IServiceCollection, Assembly[])" />
        public static IServiceCollection AddStores(this IServiceCollection serviceCollection, IEnumerable<Assembly> assemblies)
            => serviceCollection.AddStores(assemblies?.ToArray());

        /// <inheritdoc cref="AddInterfaces(IServiceCollection, Type, Assembly[])" />
        public static IServiceCollection AddStores(this IServiceCollection serviceCollection, params Assembly[] assemblies)
            => serviceCollection.AddInterfaces(DefaultStoreInterfaceType, assemblies);
        #endregion

        #region Extended Repositories
        /// <inheritdoc cref="AddExtendedRepositories(IServiceCollection, Assembly[])" />
        public static IServiceCollection AddExtendedRepositories(this IServiceCollection serviceCollection)
            => serviceCollection.AddExtendedRepositories(Assembly.GetEntryAssembly());

        /// <inheritdoc cref="AddExtendedRepositories(IServiceCollection, Assembly[])" />
        public static IServiceCollection AddExtendedRepositories(this IServiceCollection serviceCollection, IEnumerable<Assembly> assemblies)
            => serviceCollection.AddExtendedRepositories(assemblies?.ToArray());

        /// <inheritdoc cref="AddInterfaces(IServiceCollection, Type, Assembly[])" />
        public static IServiceCollection AddExtendedRepositories(this IServiceCollection serviceCollection, params Assembly[] assemblies)
            => serviceCollection.AddInterfaces(DefaultExtendedRepositoryInterfaceType, assemblies);
        #endregion

        #region Extended Stores
        /// <inheritdoc cref="AddExtendedStores(IServiceCollection, Assembly[])" />
        public static IServiceCollection AddExtendedStores(this IServiceCollection serviceCollection)
            => serviceCollection.AddExtendedStores(Assembly.GetEntryAssembly());

        /// <inheritdoc cref="AddExtendedStores(IServiceCollection, Assembly[])" />
        public static IServiceCollection AddExtendedStores(this IServiceCollection serviceCollection, IEnumerable<Assembly> assemblies)
            => serviceCollection.AddExtendedStores(assemblies?.ToArray());

        /// <inheritdoc cref="AddInterfaces(IServiceCollection, Type, Assembly[])" />
        public static IServiceCollection AddExtendedStores(this IServiceCollection serviceCollection, params Assembly[] assemblies)
            => serviceCollection.AddInterfaces(DefaultExtendedStoreInterfaceType, assemblies);
        #endregion

        /// <summary>
        /// <para>Register all `interface contract` + `concrete implementation` combos found in given assemblies.</para>
        /// <para>If single implementation is found, then it is registered regardless.</para>
        /// <para>If multiple implementations are found, only the one with the `I{Name}Repository` and `{Name}Repository` convention is registered.</para>
        /// <para>If multiple implementations are found and none has a proper name, then none is registered.</para>
        /// </summary>
        private static IServiceCollection AddInterfaces(this IServiceCollection serviceCollection, Type interfaceBaseType, params Assembly[] assemblies)
        {
            _ = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
            _ = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
            if (interfaceBaseType?.IsInterface is not true)
                throw new ArgumentException($"{nameof(interfaceBaseType)} ({interfaceBaseType.FullName}) must be an interface.", nameof(interfaceBaseType));

            //Get all contracts 
            var contracts = assemblies.SelectMany(assembly => assembly.GetTypes()).Where(type =>
            {
                //Check for matching type, excluding generic type
                static bool TypeMatches(Type leftType, Type rightType)
                {
                    _ = leftType ?? throw new ArgumentNullException(nameof(leftType));
                    _ = rightType ?? throw new ArgumentNullException(nameof(rightType));

                    static bool TypeFullNameMatches(Type leftType, Type rightType)
                    {
                        _ = leftType ?? throw new ArgumentNullException(nameof(leftType));
                        _ = rightType ?? throw new ArgumentNullException(nameof(rightType));

                        return leftType.FullName == rightType.FullName;
                    }

                    if (leftType.IsGenericType && !rightType.IsGenericType)
                        return false;
                    else if (!leftType.IsGenericType && rightType.IsGenericType)
                        return false;
                    else if (leftType.IsGenericType && rightType.IsGenericType)
                        return TypeFullNameMatches(leftType.GetGenericTypeDefinition(), rightType.GetGenericTypeDefinition());
                    else
                        return TypeFullNameMatches(leftType, rightType);
                }

                //Already registered, ignore 
                if (serviceCollection.Any(service => service.ServiceType == type))
                    return false;
                //Not an interface, ignore 
                else if (!type.IsInterface)
                    return false;
                //Doesn't implement base interface, ignore
                else if (type.GetInterface(interfaceBaseType.FullName) is null)
                    return false;
                //Found interface is not inherited directly
                else if (!type.GetDirectInterfaces().Any(directInterface => TypeMatches(directInterface, interfaceBaseType)))
                    return false;
                //Else take 
                else
                    return true;
            });

            //Find the proper implementation for each contract 
            foreach (var contract in contracts)
            {
                //Get all implementations for given contract 
                var implementations = assemblies.SelectMany(a => a.GetTypes()).Where(t =>
                {
                    //Doesn't implement given interface, ignore 
                    if (t.GetInterface(contract.FullName) is null)
                        return false;
                    //Not a class, ignore 
                    else if (!t.IsClass)
                        return false;
                    //Not a concrete class, ignore 
                    else if (t.IsAbstract)
                        return false;
                    //Else take
                    else
                        return true;
                }).ToArray();

                //If single implementation, register it 
                if (implementations.Length == 1)
                {
                    serviceCollection.AddScoped(contract, implementations[0]);
                }

                //If multiple implementations
                else if (implementations.Length > 1)
                {
                    //Local functions
                    static string GetGenericTypeBaseName(Type genericType, string name = null)
                    {
                        _ = genericType ?? throw new ArgumentNullException(nameof(genericType));
                        name ??= genericType.Name;

                        if (genericType.IsGenericType)
                            name = TrimGenericTypeName(name);

                        return name;
                    }
                    static string TrimGenericTypeName(string genericName) =>
                        genericName[..genericName.LastIndexOf('`')];

                    //Get contract name 
                    var trimmedContractName = Regex.Match(contract.Name, InterfaceContractRegexExp).Groups["Name"].Value;
                    trimmedContractName = GetGenericTypeBaseName(contract, trimmedContractName);

                    //Register only if there is a single one with the correct name convention
                    var sameNameImplementation = Array.Find(implementations, i => GetGenericTypeBaseName(i) == trimmedContractName);
                    if (sameNameImplementation is not null)
                        serviceCollection.AddScoped(contract, sameNameImplementation);
                }
            }

            return serviceCollection;
        }
    }
}

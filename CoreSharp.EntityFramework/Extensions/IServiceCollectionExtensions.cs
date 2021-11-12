﻿using CoreSharp.EntityFramework.Repositories.Interfaces;
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
        private const string RepositoryPrefix = "I";
        private const string RepositoryGroupRegexExp = "(?<Name>.+)";

        //Properties
        private static Type DefaultRepositoryInterfaceType
            => typeof(IRepository<>);

        private static string RepositoryContractRegexExp
            => $"^{RepositoryPrefix}{RepositoryGroupRegexExp}$";

        //Methods 
        /// <inheritdoc cref="AddRepositories(IServiceCollection, Type)" />
        public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
            => serviceCollection.AddRepositories(DefaultRepositoryInterfaceType);

        /// <inheritdoc cref="AddRepositories(IServiceCollection, Type, Assembly[])" />
        public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection, Type repositoryInterfaceType)
            => serviceCollection.AddRepositories(repositoryInterfaceType, Assembly.GetEntryAssembly());

        /// <inheritdoc cref="AddRepositories(IServiceCollection, Type, Assembly[])" />
        public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection, IEnumerable<Assembly> assemblies)
            => serviceCollection.AddRepositories(DefaultRepositoryInterfaceType, assemblies?.ToArray());

        /// <inheritdoc cref="AddRepositories(IServiceCollection, Type, Assembly[])" />
        public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection, params Assembly[] assembly)
            => serviceCollection.AddRepositories(DefaultRepositoryInterfaceType, assembly);

        /// <inheritdoc cref="AddRepositories(IServiceCollection, Type, Assembly[])" />
        public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection, Type repositoryInterfaceType, IEnumerable<Assembly> assemblies)
            => serviceCollection.AddRepositories(repositoryInterfaceType, assemblies?.ToArray());

        /// <summary>
        /// <para>Register all `interface contract` + `concrete implementation` combos found in given assemblies.</para>
        /// <para>If single implementation is found, then it is registered regardless.</para>
        /// <para>If multiple implementations are found, only the one with the `I{Name}Repository` and `{Name}Repository` convention is registered.</para>
        /// <para>If multiple implementations are found and none has a proper name, then none is registered.</para>
        /// </summary>
        public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection, Type repositoryInterfaceType, params Assembly[] assemblies)
        {
            _ = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
            _ = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
            if (repositoryInterfaceType?.IsInterface is not true)
                throw new ArgumentException($"{nameof(repositoryInterfaceType)} ({repositoryInterfaceType.FullName}) must be an interface.", nameof(repositoryInterfaceType));

            //Get all contracts 
            var contracts = assemblies.SelectMany(a => a.GetTypes()).Where(t =>
            {
                //Already registered, ignore 
                if (serviceCollection.Any(s => s.ServiceType == t))
                    return false;
                //Not an interface, ignore 
                else if (!t.IsInterface)
                    return false;
                //Doesn't implement base interface, ignore
                else if (t.GetInterface(repositoryInterfaceType.FullName) is null)
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
                    static string GetGenericTypeBaseName(string genericName) =>
                        genericName[..genericName.LastIndexOf('`')];
                    static string TrimGenericTypeName(Type genericType, string name = null)
                    {
                        _ = genericType ?? throw new ArgumentNullException(nameof(genericType));
                        name ??= genericType.Name;

                        if (genericType.IsGenericType)
                            name = GetGenericTypeBaseName(name);

                        return name;
                    }

                    //Get contract name 
                    var trimmedContractName = Regex.Match(contract.Name, RepositoryContractRegexExp).Groups["Name"].Value;
                    trimmedContractName = TrimGenericTypeName(contract, trimmedContractName);

                    //Register only if there is a single one with the correct name convention
                    var sameNameImplementation = Array.Find(implementations, i => TrimGenericTypeName(i) == trimmedContractName);
                    if (sameNameImplementation is not null)
                        serviceCollection.AddScoped(contract, sameNameImplementation);
                }
            }

            return serviceCollection;
        }
    }
}

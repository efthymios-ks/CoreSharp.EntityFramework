using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.UnitOfWork;
using CoreSharp.EntityFramework.Examples.CodeFirst.Domain.Database.UnitOfWork.Interfaces;
using CoreSharp.EntityFramework.Extensions;
using CoreSharp.EntityFramework.Repositories.Interfaces;
using CoreSharp.EntityFramework.Stores.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CoreSharp.EntityFramework.Examples.CodeFirst.Extensions
{
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
            _ = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));

            serviceCollection.AddRepositories(typeof(SchoolDbContext).Assembly);
            serviceCollection.AddScoped<ISchoolUnitOfWork, SchoolUnitOfWork>();

            return serviceCollection;
        }

        /// <summary>
        /// Register all <see cref="IStore{TEntity}"/>.
        /// </summary>
        public static IServiceCollection AddAppStores(this IServiceCollection serviceCollection)
        {
            _ = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));

            serviceCollection.AddStores(typeof(SchoolDbContext).Assembly);

            return serviceCollection;
        }
    }
}

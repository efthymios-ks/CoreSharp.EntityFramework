using CoreSharp.EntityFramework.Extensions;
using CoreSharp.EntityFramework.Tests.Internal.Database.Repositories;
using CoreSharp.EntityFramework.Tests.Internal.Database.Stores;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CoreSharp.EntityFramework.Tests.Extensions;

public sealed class IServiceCollectionExtensionsTests
{
    [Fact]
    public void AddRepositories_WhenCalled_ShouldRegisterRepositoriesFromGivenAssemblies()
    {
        // Arrange 
        var serviceCollection = new ServiceCollection();
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        serviceCollection.AddRepositories([assembly]);

        // Assert 
        Assert.Contains(serviceCollection, service =>
            service.ServiceType == typeof(IDummyRepository)
            && service.ImplementationType == typeof(DummyRepository)
            && service.Lifetime == ServiceLifetime.Scoped
        );
    }

    [Fact]
    public void AddExtendedRepositories_WhenCalled_ShouldRegisterExtendedRepositoriesFromGivenAssemblies()
    {
        // Arrange 
        var serviceCollection = new ServiceCollection();
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        serviceCollection.AddExtendedRepositories([assembly]);

        // Assert 
        Assert.Contains(serviceCollection, service =>
            service.ServiceType == typeof(IExtendedDummyRepository)
            && service.ImplementationType == typeof(ExtendedDummyRepository)
            && service.Lifetime == ServiceLifetime.Scoped
        );
    }

    [Fact]
    public void AddStores_WhenCalled_ShouldRegisterStoresFromGivenAssemblies()
    {
        // Arrange 
        var serviceCollection = new ServiceCollection();
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        serviceCollection.AddStores([assembly]);

        // Assert 
        Assert.Contains(serviceCollection, service =>
            service.ServiceType == typeof(IDummyStore)
            && service.ImplementationType == typeof(DummyStore)
            && service.Lifetime == ServiceLifetime.Scoped
        );
    }

    [Fact]
    public void AddExtendedStores_WhenCalled_ShouldRegisterExtendedStoresFromGivenAssemblies()
    {
        // Arrange 
        var serviceCollection = new ServiceCollection();
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        serviceCollection.AddExtendedStores([assembly]);

        // Assert 
        Assert.Contains(serviceCollection, service =>
            service.ServiceType == typeof(IExtendedDummyStore)
            && service.ImplementationType == typeof(ExtendedDummyStore)
            && service.Lifetime == ServiceLifetime.Scoped
        );
    }
}

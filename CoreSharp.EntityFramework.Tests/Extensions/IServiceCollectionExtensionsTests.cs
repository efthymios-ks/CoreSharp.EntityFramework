using CoreSharp.EntityFramework.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Tests.Internal.Database.Stores;
using Tests.Repositories.Abstracts;

namespace Tests.Extensions;

[TestFixture]
public sealed class IServiceCollectionExtensionsTests
{
    [Test]
    public void AddRepositories_WhenCalled_ShouldRegisterRepositoriesFromGivenAssemblies()
    {
        // Arrange 
        var serviceCollection = new ServiceCollection();
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        serviceCollection.AddRepositories([assembly]);

        // Assert 
        serviceCollection.Should().Contain(service =>
            service.ServiceType == typeof(IDummyRepository)
            && service.ImplementationType == typeof(DummyRepository)
            && service.Lifetime == ServiceLifetime.Scoped
        );
    }

    [Test]
    public void AddExtendedRepositories_WhenCalled_ShouldRegisterExtendedRepositoriesFromGivenAssemblies()
    {
        // Arrange 
        var serviceCollection = new ServiceCollection();
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        serviceCollection.AddExtendedRepositories([assembly]);

        // Assert 
        serviceCollection.Should().Contain(service =>
            service.ServiceType == typeof(IExtendedDummyRepository)
            && service.ImplementationType == typeof(ExtendedDummyRepository)
            && service.Lifetime == ServiceLifetime.Scoped
        );
    }

    [Test]
    public void AddStores_WhenCalled_ShouldRegisterStoresFromGivenAssemblies()
    {
        // Arrange 
        var serviceCollection = new ServiceCollection();
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        serviceCollection.AddStores([assembly]);

        // Assert 
        serviceCollection.Should().Contain(service =>
            service.ServiceType == typeof(IDummyStore)
            && service.ImplementationType == typeof(DummyStore)
            && service.Lifetime == ServiceLifetime.Scoped
        );
    }

    [Test]
    public void AddExtendedStores_WhenCalled_ShouldRegisterExtendedStoresFromGivenAssemblies()
    {
        // Arrange 
        var serviceCollection = new ServiceCollection();
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        serviceCollection.AddExtendedStores([assembly]);

        // Assert 
        serviceCollection.Should().Contain(service =>
            service.ServiceType == typeof(IExtendedDummyStore)
            && service.ImplementationType == typeof(ExtendedDummyStore)
            && service.Lifetime == ServiceLifetime.Scoped
        );
    }
}

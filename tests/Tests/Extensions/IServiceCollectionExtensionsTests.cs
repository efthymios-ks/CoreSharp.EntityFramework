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
        serviceCollection.AddRepositories(new[] { assembly });

        // Assert
        serviceCollection.Should().ContainEquivalentOf(new ServiceDescriptor(
            serviceType: typeof(IDummyRepository),
            implementationType: typeof(DummyRepository),
            lifetime: ServiceLifetime.Scoped
        ));
    }

    [Test]
    public void AddExtendedRepositories_WhenCalled_ShouldRegisterExtendedRepositoriesFromGivenAssemblies()
    {
        // Arrange 
        var serviceCollection = new ServiceCollection();
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        serviceCollection.AddExtendedRepositories(new[] { assembly });

        // Assert
        serviceCollection.Should().ContainEquivalentOf(new ServiceDescriptor(
            serviceType: typeof(IExtendedDummyRepository),
            implementationType: typeof(ExtendedDummyRepository),
            lifetime: ServiceLifetime.Scoped
        ));
    }

    [Test]
    public void AddStores_WhenCalled_ShouldRegisterStoresFromGivenAssemblies()
    {
        // Arrange 
        var serviceCollection = new ServiceCollection();
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        serviceCollection.AddStores(new[] { assembly });

        // Assert
        serviceCollection.Should().ContainEquivalentOf(new ServiceDescriptor(
            serviceType: typeof(IDummyStore),
            implementationType: typeof(DummyStore),
            lifetime: ServiceLifetime.Scoped
        ));
    }

    [Test]
    public void AddExtendedStores_WhenCalled_ShouldRegisterExtendedStoresFromGivenAssemblies()
    {
        // Arrange 
        var serviceCollection = new ServiceCollection();
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        serviceCollection.AddExtendedStores(new[] { assembly });

        // Assert
        serviceCollection.Should().ContainEquivalentOf(new ServiceDescriptor(
            serviceType: typeof(IExtendedDummyStore),
            implementationType: typeof(ExtendedDummyStore),
            lifetime: ServiceLifetime.Scoped
        ));
    }
}

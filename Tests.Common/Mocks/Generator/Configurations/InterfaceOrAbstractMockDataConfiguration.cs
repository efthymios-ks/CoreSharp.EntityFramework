namespace Tests.Common.Mocks.Generator.Configurations;

internal sealed class InterfaceOrAbstractMockDataConfiguration(Func<Type, object> abstractionFactory) : IMockDataConfiguration
{
    private readonly Func<Type, object> _abstractionFactory = abstractionFactory;

    public bool CanCreate(Type type)
        => type.IsInterface || type.IsAbstract;

    public object Create(Type type, IMockDataGenerator mockDataGenerator)
        => _abstractionFactory(type)
        ?? throw new Exception($"Unable to create an instance of {type.FullName}");
}

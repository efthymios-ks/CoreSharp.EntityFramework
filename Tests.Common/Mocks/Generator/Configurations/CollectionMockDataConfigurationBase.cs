namespace Tests.Common.Mocks.Generator.Configurations;

internal abstract class CollectionMockDataConfigurationBase : IMockDataConfiguration
{
    public const int CollectionMin = 2;
    public const int CollectionMax = 6;

    public abstract bool CanCreate(Type type);
    public abstract object Create(Type type, IMockDataGenerator mockDataGenerator);
}

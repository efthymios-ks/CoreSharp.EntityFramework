namespace Tests.Common.Mocks.Generator;

public interface IMockDataConfiguration
{
    bool CanCreate(Type type);
    object Create(Type type, IMockDataGenerator mockDataGenerator);
}
